
namespace Application;
public interface IMessageService
{
    Task<List<MessageDto>> GetPagedMessagesAsync(Guid conversationId, int page, int pageSize, CancellationToken ct = default);
    Task<MessageDto> SendMessageAsync(Guid conversationId, CreateMessageRequest request, CancellationToken ct = default);
    Task<MessageDto> EditMessageAsync(Guid messageId, string newContent, CancellationToken ct = default);
    Task DeleteMessageAsync(Guid messageId, CancellationToken ct = default);
    Task<MessageReactionDto> ToggleReactionAsync(Guid messageId, string reactionType, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid conversationId, CancellationToken ct = default);
}
