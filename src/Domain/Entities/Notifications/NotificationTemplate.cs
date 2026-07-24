namespace Domain;

public class NotificationTemplate : EntityBase<Guid>
{
    public Guid EventTypeId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string TitleTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public NotificationEventType? EventType { get; set; }
}
