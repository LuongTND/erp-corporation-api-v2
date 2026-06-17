using Application.Common.Exceptions;
using Application.DTOs.Chat;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Chat;
using Application.Interfaces.Services.Chat;
using Application.Interfaces.Services.Auth;
using AutoMapper;
using Domain.Entities.Chat;
using Domain.Enums.Chat;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Services.Chat;

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

        var lastMessages = await LoadLastMessagesAsync([id], ct);
        if (lastMessages.TryGetValue(id, out var lastMsg))
            dto.LastMessage = _mapper.Map<MessageDto>(lastMsg);

        if (member != null)
            dto.UnreadCount = await CalculateUnreadCountAsync(id, currentUserId, member.LastReadMessageID, ct);

        return dto;
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var conversations = await _conversationRepository.GetUserConversationsAsync(currentUserId, ct);
        if (conversations.Count == 0)
            return [];

        var convIds = conversations.Select(c => c.Id).ToList();
        var lastMessages = await LoadLastMessagesAsync(convIds, ct);
        var unreadCounts = await LoadUnreadCountsAsync(currentUserId, conversations, ct);

        var dtos = new List<ConversationDto>(conversations.Count);
        foreach (var conv in conversations)
        {
            var dto = _mapper.Map<ConversationDto>(conv);

            if (lastMessages.TryGetValue(conv.Id, out var lastMsg))
                dto.LastMessage = _mapper.Map<MessageDto>(lastMsg);

            if (unreadCounts.TryGetValue(conv.Id, out var unread))
                dto.UnreadCount = unread;

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
        conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId, ConversationActivityAction.MemberJoined, $"Người tạo tạo cuộc hội thoại và tham gia"));

        await _conversationRepository.AddAsync(conv, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(conv.Id, ct);
    }

    public async Task<ConversationDto> GetOrCreateDirectConversationAsync(Guid otherUserId, CancellationToken ct = default)
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
        conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId, isArchived ? ConversationActivityAction.Archived : ConversationActivityAction.Unarchived));

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
            conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId, ConversationActivityAction.MemberLeft, $"Thành viên {userId} rời/bị xóa khỏi cuộc hội thoại."));
            
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
                    conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId, ConversationActivityAction.MemberJoined, $"Thành viên {userId} quay lại cuộc hội thoại."));
                }
            }
            else
            {
                conv.Members.Add(ConversationMember.Create(conv.Id, userId, RoleInConversation.Member));
                conv.ActivityLogs.Add(ConversationActivityLog.Create(conv.Id, currentUserId, ConversationActivityAction.MemberJoined, $"Thành viên {userId} được thêm vào cuộc hội thoại."));
            }
        }

        _conversationRepository.Update(conv);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Dictionary<Guid, Message>> LoadLastMessagesAsync(IReadOnlyList<Guid> conversationIds, CancellationToken ct)
    {
        if (conversationIds.Count == 0)
            return [];

        var lastMessageIds = await _messageRepository.GetQueryable()
            .AsNoTracking()
            .Where(m => conversationIds.Contains(m.ConversationID) && !m.IsDeleted)
            .GroupBy(m => m.ConversationID)
            .Select(g => g.OrderByDescending(m => m.CreatedAt).Select(m => m.Id).First())
            .ToListAsync(ct);

        if (lastMessageIds.Count == 0)
            return [];

        var messages = await _messageRepository.GetQueryable()
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
                .ThenInclude(r => r.User)
            .Where(m => lastMessageIds.Contains(m.Id))
            .AsSplitQuery()
            .ToListAsync(ct);

        return messages.ToDictionary(m => m.ConversationID);
    }

    private async Task<Dictionary<Guid, int>> LoadUnreadCountsAsync(
        Guid currentUserId,
        IReadOnlyList<Conversation> conversations,
        CancellationToken ct)
    {
        var result = new Dictionary<Guid, int>();
        var convIds = conversations.Select(c => c.Id).ToList();
        if (convIds.Count == 0)
            return result;

        var memberships = conversations
            .Select(c => (ConvId: c.Id, Member: c.Members.FirstOrDefault(m => m.UserID == currentUserId && m.IsActive)))
            .Where(x => x.Member != null)
            .ToList();

        var lastReadIds = memberships
            .Where(x => x.Member!.LastReadMessageID.HasValue)
            .Select(x => x.Member!.LastReadMessageID!.Value)
            .Distinct()
            .ToList();

        var thresholdByConv = lastReadIds.Count == 0
            ? new Dictionary<Guid, DateTime>()
            : (await _messageRepository.GetQueryable()
                .AsNoTracking()
                .Where(m => lastReadIds.Contains(m.Id))
                .Select(m => new { m.ConversationID, m.CreatedAt })
                .ToListAsync(ct))
                .ToDictionary(x => x.ConversationID, x => x.CreatedAt);

        var unreadCandidates = await _messageRepository.GetQueryable()
            .AsNoTracking()
            .Where(m => convIds.Contains(m.ConversationID) && !m.IsDeleted && m.UserID != currentUserId)
            .Select(m => new { m.ConversationID, m.CreatedAt })
            .ToListAsync(ct);

        foreach (var (convId, member) in memberships)
        {
            DateTime? threshold = null;
            if (member!.LastReadMessageID.HasValue && thresholdByConv.TryGetValue(convId, out var ts))
                threshold = ts;

            result[convId] = unreadCandidates.Count(m =>
                m.ConversationID == convId &&
                (!threshold.HasValue || m.CreatedAt > threshold.Value));
        }

        return result;
    }

    private async Task<int> CalculateUnreadCountAsync(Guid conversationId, Guid userId, Guid? lastReadMessageId, CancellationToken ct)
    {
        DateTime? threshold = null;
        if (lastReadMessageId.HasValue)
        {
            threshold = await _messageRepository.GetQueryable()
                .AsNoTracking()
                .Where(m => m.Id == lastReadMessageId.Value)
                .Select(m => (DateTime?)m.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        return await _messageRepository.GetQueryable()
            .AsNoTracking()
            .CountAsync(m => m.ConversationID == conversationId &&
                             !m.IsDeleted &&
                             m.UserID != userId &&
                             (!threshold.HasValue || m.CreatedAt > threshold.Value), ct);
    }
}
