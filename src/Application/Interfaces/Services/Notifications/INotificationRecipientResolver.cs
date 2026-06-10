using Application.DTOs.Notifications;

namespace Application.Interfaces.Services.Notifications;

public interface INotificationRecipientResolver
{
    Task<IReadOnlyList<Guid>> ResolveAsync(
        NotificationRecipientRulesDto rules,
        NotificationPublishContext context,
        CancellationToken cancellationToken = default);
}
