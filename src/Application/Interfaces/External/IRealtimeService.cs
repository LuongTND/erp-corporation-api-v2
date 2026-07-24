namespace Application;

public interface IChatRealtimeService
{
    Task SendMessageAsync(string conversationId, object payload, CancellationToken ct = default);
    Task SendTypingStatusAsync(string conversationId, string userId, string fullName, bool isTyping, CancellationToken ct = default);
}

public interface INotificationRealtimeService
{
    Task SendToUserAsync(Guid userId, object payload, CancellationToken ct = default);
}
