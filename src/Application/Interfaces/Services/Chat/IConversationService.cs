
namespace Application;
public interface IConversationService
{
    Task<ConversationDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ConversationDto>> GetUserConversationsAsync(CancellationToken ct = default);
    Task<ConversationDto> CreateAsync(CreateConversationRequest request, CancellationToken ct = default);
    Task<ConversationDto> GetOrCreateDirectConversationAsync(Guid otherUserId, CancellationToken ct = default);
    Task SetMuteAsync(Guid conversationId, bool isMuted, CancellationToken ct = default);
    Task SetArchivedAsync(Guid conversationId, bool isArchived, CancellationToken ct = default);
    Task RemoveMemberAsync(Guid conversationId, Guid userId, CancellationToken ct = default);
    Task AddMembersAsync(Guid conversationId, List<Guid> userIds, CancellationToken ct = default);
}
