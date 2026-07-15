
namespace Domain;
public class NotificationTemplate : BaseEntity, IAuditable
{
    public Guid EventTypeId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string TitleTemplate { get; private set; } = null!;
    public string BodyTemplate { get; private set; } = null!;
    public bool IsActive { get; set; } = true;

    public virtual NotificationEventType EventType { get; private set; } = null!;

    private NotificationTemplate() : base()
    {
    }

    public static NotificationTemplate Create(
        Guid eventTypeId,
        NotificationChannel channel,
        string titleTemplate,
        string bodyTemplate)
    {
        return new NotificationTemplate
        {
            EventTypeId = eventTypeId,
            Channel = channel,
            TitleTemplate = titleTemplate,
            BodyTemplate = bodyTemplate,
            IsActive = true
        };
    }

    public void Update(string titleTemplate, string bodyTemplate, bool isActive)
    {
        TitleTemplate = titleTemplate;
        BodyTemplate = bodyTemplate;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
