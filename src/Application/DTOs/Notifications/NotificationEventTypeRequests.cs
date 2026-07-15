namespace Application;

public record CreateNotificationEventTypeRequest
{
    public string EventCode { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Module { get; init; } = null!;
    public string? Description { get; init; }
    public string DefaultTitleTemplate { get; init; } = null!;
    public string DefaultBodyTemplate { get; init; } = null!;
}

public record UpdateNotificationEventTypeRequest
{
    public string Name { get; init; } = null!;
    public string Module { get; init; } = null!;
    public string? Description { get; init; }
    public string DefaultTitleTemplate { get; init; } = null!;
    public string DefaultBodyTemplate { get; init; } = null!;
    public bool IsActive { get; init; }
}

public record NotificationTemplateDto
{
    public Guid Id { get; init; }
    public Guid EventTypeId { get; init; }
    public NotificationChannel Channel { get; init; }
    public string TitleTemplate { get; init; } = null!;
    public string BodyTemplate { get; init; } = null!;
    public bool IsActive { get; init; }
}

public record UpsertNotificationTemplateRequest
{
    public string TitleTemplate { get; init; } = null!;
    public string BodyTemplate { get; init; } = null!;
    public bool IsActive { get; init; } = true;
}