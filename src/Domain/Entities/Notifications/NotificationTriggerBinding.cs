namespace Domain;

public class NotificationTriggerBinding : BaseEntity
{
    public string TriggerKey { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Module { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? EventTypeId { get; private set; }
    public string? LinkUrlTemplate { get; private set; }
    public string RecipientRulesJson { get; private set; } = "{}";
    public bool IsActive { get; private set; } = true;

    public virtual NotificationEventType? EventType { get; private set; }

    private NotificationTriggerBinding() : base()
    {
    }

    public static NotificationTriggerBinding Create(
        Guid id,
        string triggerKey,
        string name,
        string module,
        Guid? eventTypeId,
        string? linkUrlTemplate,
        string? description = null,
        string? recipientRulesJson = null)
    {
        return new NotificationTriggerBinding
        {
            Id = id,
            TriggerKey = triggerKey.ToLowerInvariant(),
            Name = name,
            Module = module,
            EventTypeId = eventTypeId,
            LinkUrlTemplate = linkUrlTemplate,
            Description = description,
            RecipientRulesJson = recipientRulesJson ?? "{}",
            IsActive = true
        };
    }

    public void AssignEventType(Guid? eventTypeId, string? linkUrlTemplate, bool isActive, string recipientRulesJson)
    {
        EventTypeId = eventTypeId;
        LinkUrlTemplate = linkUrlTemplate;
        IsActive = isActive;
        RecipientRulesJson = recipientRulesJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRecipientRulesJson(string recipientRulesJson)
    {
        RecipientRulesJson = recipientRulesJson;
        UpdatedAt = DateTime.UtcNow;
    }
}