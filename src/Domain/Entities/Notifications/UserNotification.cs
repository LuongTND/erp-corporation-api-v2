namespace Domain;

public class UserNotification : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public string TriggerKey { get; set; } = string.Empty;
    public Guid EventTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public User? User { get; set; }
    public NotificationEventType? EventType { get; set; }

    public void MarkRead()
    {
        if (IsRead) return;

        IsRead = true;
        ReadAt = DateTimeOffset.UtcNow;
    }
}
