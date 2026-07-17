namespace Infrastructure;

/// <summary>
/// Fallback khi SignalR chưa được đăng ký (tests). API ghi đè bằng <see cref="API.Services.SignalRNotificationSender"/>.
/// </summary>
[RegisterService(typeof(INotificationRealtimeSender))]
public class NoOpNotificationRealtimeSender : INotificationRealtimeSender
{
    public Task SendToUserAsync(Guid userId, RealtimeNotificationPayload payload,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}