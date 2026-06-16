using Domain.Entities.Chat;

namespace Application.Interfaces.Repositories.Chat;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
    Task<List<Conversation>> GetUserConversationsAsync(Guid userId, CancellationToken ct = default);
    Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken ct = default);
}
