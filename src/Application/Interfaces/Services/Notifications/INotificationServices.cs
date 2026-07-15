
namespace Application;
public interface INotificationEventTypeService : ICrudService<NotificationEventTypeDto, CreateNotificationEventTypeRequest, UpdateNotificationEventTypeRequest>
{
    Task<IReadOnlyList<NotificationTemplateDto>> GetTemplatesAsync(Guid eventTypeId, CancellationToken ct = default);
    Task<NotificationTemplateDto> UpsertTemplateAsync(Guid eventTypeId, NotificationChannel channel, UpsertNotificationTemplateRequest request, CancellationToken ct = default);
}

public interface INotificationTriggerService
{
    Task<PaginatedResult<NotificationTriggerBindingDto>> GetPagedAsync(PaginationQuery query, string? module, CancellationToken ct = default);
    Task<NotificationTriggerBindingDto> GetByTriggerKeyAsync(string triggerKey, CancellationToken ct = default);
    Task<NotificationTriggerBindingDto> UpdateBindingAsync(string triggerKey, UpdateNotificationTriggerBindingRequest request, CancellationToken ct = default);
}

public interface IUserNotificationService
{
    Task<PaginatedResult<UserNotificationDto>> GetMyPagedAsync(PaginationQuery query, bool? isRead, CancellationToken ct = default);
    Task<UnreadNotificationCountDto> GetMyUnreadCountAsync(CancellationToken ct = default);
    Task<UserNotificationDto> MarkReadAsync(Guid id, CancellationToken ct = default);
    Task MarkAllReadAsync(CancellationToken ct = default);
}

public interface INotificationPublisher
{
    Task PublishAsync(
        string triggerKey,
        NotificationPublishContext context,
        object data,
        string? linkUrlOverride = null,
        CancellationToken cancellationToken = default);
}

public interface INotificationRealtimeSender
{
    Task SendToUserAsync(Guid userId, RealtimeNotificationPayload payload, CancellationToken cancellationToken = default);
}
