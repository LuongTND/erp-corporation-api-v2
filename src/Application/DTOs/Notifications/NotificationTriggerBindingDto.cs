namespace Application;

public record NotificationTriggerBindingDto : IHasGuidId
{
    public Guid Id { get; init; }
    public string TriggerKey { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Module { get; init; } = null!;
    public string? Description { get; init; }
    public Guid? EventTypeId { get; init; }
    public string? EventCode { get; init; }
    public string? EventTypeName { get; init; }
    public string? LinkUrlTemplate { get; init; }
    public NotificationRecipientRulesDto RecipientRules { get; init; } = new();
    public bool IsActive { get; init; }
}

public record UpdateNotificationTriggerBindingRequest
{
    public Guid? EventTypeId { get; init; }
    public string? LinkUrlTemplate { get; init; }
    public NotificationRecipientRulesDto? RecipientRules { get; init; }
    public bool IsActive { get; init; } = true;
}