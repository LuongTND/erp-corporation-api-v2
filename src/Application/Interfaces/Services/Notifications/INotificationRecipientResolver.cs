
namespace Application;
public interface INotificationRecipientResolver
{
    Task<IReadOnlyList<Guid>> ResolveAsync(
        NotificationRecipientRulesDto rules,
        NotificationPublishContext context,
        CancellationToken cancellationToken = default);
}
