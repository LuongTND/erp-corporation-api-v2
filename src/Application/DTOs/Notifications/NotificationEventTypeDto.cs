namespace Application;

public record NotificationEventTypeDto : IHasGuidId
{
    public Guid Id { get; init; }
    public string EventCode { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Module { get; init; } = null!;
    public string? Description { get; init; }
    public string DefaultTitleTemplate { get; init; } = null!;
    public string DefaultBodyTemplate { get; init; } = null!;
    public bool IsActive { get; init; }
}