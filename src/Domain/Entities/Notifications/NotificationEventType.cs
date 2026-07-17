namespace Domain;

public class NotificationEventType : BaseEntity, IAuditable
{
    public string EventCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Module { get; private set; } = null!;
    public string? Description { get; private set; }
    public string DefaultTitleTemplate { get; private set; } = null!;
    public string DefaultBodyTemplate { get; private set; } = null!;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<NotificationTemplate> Templates { get; private set; } = [];
    public virtual ICollection<NotificationTriggerBinding> TriggerBindings { get; private set; } = [];

    private NotificationEventType() : base()
    {
    }

    public static NotificationEventType Create(
        Guid id,
        string eventCode,
        string name,
        string module,
        string defaultTitleTemplate,
        string defaultBodyTemplate,
        string? description = null)
    {
        return new NotificationEventType
        {
            Id = id,
            EventCode = eventCode.ToLowerInvariant(),
            Name = name,
            Module = module,
            Description = description,
            DefaultTitleTemplate = defaultTitleTemplate,
            DefaultBodyTemplate = defaultBodyTemplate,
            IsActive = true
        };
    }

    public void Update(
        string name,
        string module,
        string defaultTitleTemplate,
        string defaultBodyTemplate,
        string? description,
        bool isActive)
    {
        Name = name;
        Module = module;
        DefaultTitleTemplate = defaultTitleTemplate;
        DefaultBodyTemplate = defaultBodyTemplate;
        Description = description;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}