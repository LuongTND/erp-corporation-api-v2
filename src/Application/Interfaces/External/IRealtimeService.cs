namespace Application;

public interface INotificationRealtimeService
{
    Task SendToUserAsync(Guid userId, object payload, CancellationToken ct = default);
}
