
namespace Application;
public interface INotificationActorResolver
{
    Task<string> GetActorDisplayNameAsync(CancellationToken ct = default);

    NotificationPublishContext BuildContext(Guid? subjectUserId = null);
}
