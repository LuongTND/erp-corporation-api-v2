namespace Domain;

public class UserNotification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TriggerKey { get; private set; } = null!;
    public Guid EventTypeId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string? LinkUrl { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public virtual User User { get; private set; } = null!;
    public virtual NotificationEventType EventType { get; private set; } = null!;

    private UserNotification() : base()
    {
    }

    public static UserNotification Create(
        Guid userId,
        string triggerKey,
        Guid eventTypeId,
        string title,
        string body,
        string? linkUrl)
    {
        return new UserNotification
        {
            UserId = userId,
            TriggerKey = triggerKey,
            EventTypeId = eventTypeId,
            Title = title,
            Body = body,
            LinkUrl = linkUrl,
            IsRead = false
        };
    }

    public void MarkRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}