using Application.Common.Models;

namespace Application.DTOs.Notifications;

public record UserNotificationDto : IHasGuidId
{
    public Guid Id { get; init; }
    public string TriggerKey { get; init; } = null!;
    public Guid EventTypeId { get; init; }
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
    public string? LinkUrl { get; init; }
    public bool IsRead { get; init; }
    public DateTime? ReadAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record UnreadNotificationCountDto
{
    public int Count { get; init; }
}

public record RealtimeNotificationPayload
{
    public UserNotificationDto Notification { get; init; } = null!;
    public int UnreadCount { get; init; }
}
