namespace Domain;

public class NotificationEventType : EntityBase<Guid>
{
    public string EventCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DefaultTitleTemplate { get; set; } = string.Empty;
    public string DefaultBodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<NotificationTemplate> Templates { get; set; } = [];
    public ICollection<NotificationTriggerBinding> TriggerBindings { get; set; } = [];
}
