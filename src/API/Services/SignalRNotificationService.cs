namespace API;

[RegisterService(typeof(INotificationRealtimeService))]
public sealed class SignalRNotificationService(IHubContext<NotificationHub> hub) : INotificationRealtimeService
{
    public async Task SendToUserAsync(Guid userId, object payload, CancellationToken ct = default)
        => await hub.Clients.User(userId.ToString()).SendAsync(NotificationHub.ReceiveNotificationMethod, payload, ct);
}
