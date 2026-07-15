
namespace Application;
public interface INotificationEventTypeRepository : IGenericRepository<NotificationEventType>
{
    Task<PaginatedResult<NotificationEventType>> GetPagedAsync(PaginationQuery query, string? module, bool? isActive, CancellationToken ct = default);
    Task<NotificationEventType?> GetByCodeAsync(string eventCode, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string eventCode, CancellationToken ct = default);
    Task<bool> ExistsByCodeExcludeIdAsync(string eventCode, Guid excludeId, CancellationToken ct = default);
    Task<bool> HasActiveTriggerBindingsAsync(Guid eventTypeId, CancellationToken ct = default);
}

public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetByEventTypeAndChannelAsync(Guid eventTypeId, NotificationChannel channel, CancellationToken ct = default);
    Task<NotificationTemplate?> FindByEventTypeAndChannelAsync(Guid eventTypeId, NotificationChannel channel, CancellationToken ct = default);
    Task<IReadOnlyList<NotificationTemplate>> GetByEventTypeIdAsync(Guid eventTypeId, CancellationToken ct = default);
}

public interface INotificationTriggerBindingRepository : IGenericRepository<NotificationTriggerBinding>
{
    Task<PaginatedResult<NotificationTriggerBinding>> GetPagedAsync(PaginationQuery query, string? module, CancellationToken ct = default);
    Task<NotificationTriggerBinding?> GetByTriggerKeyAsync(string triggerKey, CancellationToken ct = default);
    Task<NotificationTriggerBinding?> GetByTriggerKeyWithEventTypeAsync(string triggerKey, CancellationToken ct = default);
}

public interface IUserNotificationRepository : IGenericRepository<UserNotification>
{
    Task<PaginatedResult<UserNotification>> GetPagedForUserAsync(Guid userId, PaginationQuery query, bool? isRead, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<UserNotification?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task MarkAllReadAsync(Guid userId, CancellationToken ct = default);
}
