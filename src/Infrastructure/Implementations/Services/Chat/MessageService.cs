namespace Infrastructure;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public MessageService(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        AppDbContext context)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<MessageDto>> GetPagedMessagesAsync(Guid conversationId, int page, int pageSize,
        CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null && conv.IsPrivate)
            throw new ForbiddenException("Bạn không có quyền xem tin nhắn của cuộc hội thoại này.");

        var messages = await _messageRepository.GetPagedMessagesAsync(conversationId, page, pageSize, ct);
        return _mapper.Map<List<MessageDto>>(messages);
    }

    public async Task<MessageDto> SendMessageAsync(Guid conversationId, CreateMessageRequest request,
        CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        var message = Message.Create(
            conversationId,
            currentUserId,
            request.Content,
            request.MessageType,
            request.ParentMessageID
        );

        // Attachments
        if (request.Attachments != null)
        {
            foreach (var att in request.Attachments)
            {
                message.Attachments.Add(MessageAttachment.Create(
                    message.Id,
                    att.FileName,
                    att.FileURL,
                    att.FileType,
                    att.FileSize,
                    att.ThumbnailURL
                ));
            }
        }

        await _messageRepository.AddAsync(message, ct);

        // Set LastReadMessageID for the sender
        member.UpdateLastReadMessage(message.Id);
        _conversationRepository.Update(conv);

        await _unitOfWork.SaveChangesAsync(ct);

        // Load message details with User entity populated for mapping
        var msgDb = await _messageRepository.GetQueryable()
            .Include(m => m.User)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
            .ThenInclude(r => r.User)
            .Include(m => m.MessageTasks)
            .FirstOrDefaultAsync(m => m.Id == message.Id, ct);

        return _mapper.Map<MessageDto>(msgDb);
    }

    public async Task<MessageDto> EditMessageAsync(Guid messageId, string newContent, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var message = await _messageRepository.GetQueryable()
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Members)
            .FirstOrDefaultAsync(m => m.Id == messageId, ct);

        if (message == null || message.IsDeleted)
            throw new NotFoundException("Không tìm thấy tin nhắn cần sửa.");

        if (message.UserID != currentUserId)
            throw new ForbiddenException("Bạn chỉ có thể sửa tin nhắn của chính mình.");

        // Edit message
        message.Edit(newContent);
        _messageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync(ct);

        // Fetch again with details
        var msgDb = await _messageRepository.GetQueryable()
            .Include(m => m.User)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
            .ThenInclude(r => r.User)
            .Include(m => m.MessageTasks)
            .FirstOrDefaultAsync(m => m.Id == messageId, ct);

        return _mapper.Map<MessageDto>(msgDb);
    }

    public async Task DeleteMessageAsync(Guid messageId, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var message = await _messageRepository.GetQueryable()
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Members)
            .FirstOrDefaultAsync(m => m.Id == messageId, ct);

        if (message == null || message.IsDeleted)
            throw new NotFoundException("Không tìm thấy tin nhắn cần xóa.");

        var member = message.Conversation.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        // Sender or Admin of the conversation can delete message
        if (message.UserID != currentUserId && member.RoleInConversation != RoleInConversation.Admin)
            throw new ForbiddenException("Bạn không có quyền xóa tin nhắn này.");

        message.Delete();
        _messageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<MessageReactionDto> ToggleReactionAsync(Guid messageId, string reactionType,
        CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var message = await _messageRepository.GetQueryable()
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Members)
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.Id == messageId, ct);

        if (message == null || message.IsDeleted)
            throw new NotFoundException("Không tìm thấy tin nhắn.");

        var member = message.Conversation.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        var existingReaction =
            message.Reactions.FirstOrDefault(r => r.UserID == currentUserId && r.ReactionType == reactionType);

        if (existingReaction != null)
        {
            // Remove reaction
            _context.MessageReactions.Remove(existingReaction);
            await _unitOfWork.SaveChangesAsync(ct);
            return new MessageReactionDto
            {
                MessageID = messageId,
                UserID = currentUserId,
                ReactionType = string.Empty
            };
        }
        else
        {
            // Add reaction
            var newReaction = MessageReaction.Create(messageId, currentUserId, reactionType);
            await _context.MessageReactions.AddAsync(newReaction, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Fetch reaction with user details safely and bypass EF tracking cache
            var messageWithReactions = await _messageRepository.GetQueryable()
                .AsNoTracking()
                .Include(m => m.Reactions)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == messageId, ct);

            var reactionDb = messageWithReactions?.Reactions.FirstOrDefault(r => r.Id == newReaction.Id);

            return _mapper.Map<MessageReactionDto>(reactionDb);
        }
    }

    public async Task MarkAsReadAsync(Guid conversationId, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        // Find the latest message in this conversation
        var latestMsg = await _messageRepository.GetQueryable()
            .Where(m => m.ConversationID == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (latestMsg != null)
        {
            member.UpdateLastReadMessage(latestMsg.Id);
            _conversationRepository.Update(conv);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}