namespace Application;

public sealed record PermissionAuditLogFilter
{
    public string? Action { get; init; }
    public Guid? ActorId { get; init; }
    public Guid? RoleId { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public int Top { get; init; } = 15;
    public int Skip { get; init; } = 0;
}
