using API.Hubs;
using Application.DTOs.Notifications;
using Application.Interfaces.Services.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

public class SignalRNotificationSender : INotificationRealtimeSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(Guid userId, RealtimeNotificationPayload payload, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync(NotificationHub.ReceiveNotificationMethod, payload, cancellationToken);

        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync(NotificationHub.UnreadCountUpdatedMethod, payload.UnreadCount, cancellationToken);
    }
}
