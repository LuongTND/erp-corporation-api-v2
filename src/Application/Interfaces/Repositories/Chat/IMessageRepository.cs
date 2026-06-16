using Domain.Entities.Chat;

namespace Application.Interfaces.Repositories.Chat;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetPagedMessagesAsync(Guid conversationId, int page, int pageSize, CancellationToken ct = default);
}
