namespace Domain;

public class NotificationTriggerBinding : EntityBase<Guid>
{
    public string TriggerKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? EventTypeId { get; set; }
    public string? LinkUrlTemplate { get; set; }
    public string RecipientRulesJson { get; set; } = "{}";
    public bool IsActive { get; set; } = true;

    public NotificationEventType? EventType { get; set; }
}
