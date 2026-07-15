namespace Infrastructure;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ConversationService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ConversationDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conv = await _conversationRepository.GetByIdWithMembersAsync(id, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null && conv.IsPrivate)
            throw new ForbiddenException("Bạn không có quyền truy cập cuộc hội thoại này.");

        var dto = _mapper.Map<ConversationDto>(conv);

        // Fetch last message
        var lastMsg = await _messageRepository.GetQueryable()
            .Include(m => m.User)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
            .ThenInclude(r => r.User)
            .Where(m => m.ConversationID == id && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (lastMsg != null)
        {
            dto.LastMessage = _mapper.Map<MessageDto>(lastMsg);
        }

        // Fetch unread count for current user
        if (member != null)
        {
            dto.UnreadCount = await CalculateUnreadCountAsync(id, currentUserId, member.LastReadMessageID, ct);
        }

        return dto;
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conversations = await _conversationRepository.GetUserConversationsAsync(currentUserId, ct);
        var dtos = new List<ConversationDto>();

        foreach (var conv in conversations)
        {
            var dto = _mapper.Map<ConversationDto>(conv);

            // Fetch last message
            var lastMsg = await _messageRepository.GetQueryable()
                .Include(m => m.User)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .ThenInclude(r => r.User)
                .Where(m => m.ConversationID == conv.Id && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (lastMsg != null)
            {
                dto.LastMessage = _mapper.Map<MessageDto>(lastMsg);
            }

            var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
            if (member != null)
            {
                dto.UnreadCount = await CalculateUnreadCountAsync(conv.Id, currentUserId, member.LastReadMessageID, ct);
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<ConversationDto> CreateAsync(CreateConversationRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conv = Conversation.Create(
            request.ConversationType,
            request.Title,
            request.Description,
            request.IsPrivate,
            currentUserId
        );

        // Add creator as Admin
        conv.Members.Add(ConversationMember.Create(conv.Id, currentUserId, RoleInConversation.Admin));

        // Add other members
        var distinctMemberIds = request.MemberIds.Where(id => id != currentUserId).Distinct();
        foreach (var memberId in distinctMemberIds)
        {
            conv.Members.Add(ConversationMember.Create(conv.Id, memberId, RoleInConversation.Member));
        }

        // Log activity
        conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId,
            ConversationActivityAction.MemberJoined, $"Người tạo tạo cuộc hội thoại và tham gia"));

        await _conversationRepository.AddAsync(conv, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(conv.Id, ct);
    }

    public async Task<ConversationDto> GetOrCreateDirectConversationAsync(Guid otherUserId,
        CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == otherUserId)
            throw new ArgumentException("Không thể tạo cuộc trò chuyện 1-1 với chính mình.");

        var existing = await _conversationRepository.GetDirectConversationAsync(currentUserId, otherUserId, ct);
        if (existing != null)
        {
            return await GetByIdAsync(existing.Id, ct);
        }

        var conv = Conversation.Create(
            ConversationType.Direct,
            null,
            null,
            true,
            currentUserId
        );

        conv.Members.Add(ConversationMember.Create(conv.Id, currentUserId, RoleInConversation.Member));
        conv.Members.Add(ConversationMember.Create(conv.Id, otherUserId, RoleInConversation.Member));

        await _conversationRepository.AddAsync(conv, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(conv.Id, ct);
    }

    public async Task SetMuteAsync(Guid conversationId, bool isMuted, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        member.SetMuted(isMuted);
        _conversationRepository.Update(conv);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task SetArchivedAsync(Guid conversationId, bool isArchived, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var member = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (member == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        conv.SetArchived(isArchived);
        conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId,
            isArchived ? ConversationActivityAction.Archived : ConversationActivityAction.Unarchived));

        _conversationRepository.Update(conv);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveMemberAsync(Guid conversationId, Guid userId, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var currentMember = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (currentMember == null)
            throw new ForbiddenException("Bạn không phải thành viên của cuộc hội thoại này.");

        // Member is leaving or admin is kicking
        if (currentUserId != userId && currentMember.RoleInConversation != RoleInConversation.Admin)
            throw new ForbiddenException("Chỉ quản trị viên mới có quyền xóa thành viên.");

        var targetMember = conv.Members.FirstOrDefault(m => m.UserID == userId && m.IsActive);
        if (targetMember != null)
        {
            targetMember.Deactivate();
            conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId,
                ConversationActivityAction.MemberLeft, $"Thành viên {userId} rời/bị xóa khỏi cuộc hội thoại."));

            _conversationRepository.Update(conv);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task AddMembersAsync(Guid conversationId, List<Guid> userIds, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var conv = await _conversationRepository.GetByIdWithMembersAsync(conversationId, ct);
        if (conv == null || !conv.IsActive)
            throw new NotFoundException("Không tìm thấy cuộc hội thoại.");

        var currentMember = conv.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive);
        if (currentMember == null && conv.IsPrivate)
            throw new ForbiddenException("Bạn không có quyền thêm thành viên vào cuộc hội thoại này.");

        foreach (var userId in userIds)
        {
            var existingMember = conv.Members.FirstOrDefault(m => m.UserID == userId);
            if (existingMember != null)
            {
                if (!existingMember.IsActive)
                {
                    existingMember.Reactivate();
                    conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId,
                        ConversationActivityAction.MemberJoined, $"Thành viên {userId} quay lại cuộc hội thoại."));
                }
            }
            else
            {
                conv.Members.Add(ConversationMember.Create(conv.Id, userId, RoleInConversation.Member));
                conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId,
                    ConversationActivityAction.MemberJoined, $"Thành viên {userId} được thêm vào cuộc hội thoại."));
            }
        }

        _conversationRepository.Update(conv);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<int> CalculateUnreadCountAsync(Guid conversationId, Guid userId, Guid? lastReadMessageId,
        CancellationToken ct)
    {
        if (lastReadMessageId.HasValue)
        {
            var lastReadMsg = await _messageRepository.GetByIdAsync(lastReadMessageId.Value, ct);
            if (lastReadMsg != null)
            {
                return await _messageRepository.GetQueryable()
                    .CountAsync(m => m.ConversationID == conversationId &&
                                     m.CreatedAt > lastReadMsg.CreatedAt &&
                                     m.UserID != userId &&
                                     !m.IsDeleted, ct);
            }
        }

        return await _messageRepository.GetQueryable()
            .CountAsync(m => m.ConversationID == conversationId &&
                             m.UserID != userId &&
                             !m.IsDeleted, ct);
    }
}