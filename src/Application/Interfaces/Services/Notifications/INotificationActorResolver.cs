using Application.DTOs.Notifications;

namespace Application.Interfaces.Services.Notifications;

public interface INotificationActorResolver
{
    Task<string> GetActorDisplayNameAsync(CancellationToken ct = default);

    NotificationPublishContext BuildContext(Guid? subjectUserId = null);
}
