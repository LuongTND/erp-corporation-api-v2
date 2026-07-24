namespace API;

[RegisterService(typeof(IChatRealtimeService))]
public sealed class SignalRChatService(IHubContext<ChatHub> hub) : IChatRealtimeService
{
    public async Task SendMessageAsync(string conversationId, object payload, CancellationToken ct = default)
        => await hub.Clients.Group(conversationId).SendAsync("ReceiveMessage", payload, ct);

    public async Task SendTypingStatusAsync(string conversationId, string userId, string fullName, bool isTyping, CancellationToken ct = default)
        => await hub.Clients.Group(conversationId).SendAsync("ReceiveTypingStatus", conversationId, userId, fullName, isTyping, ct);
}
