namespace Application;

public sealed record PermissionAuditLogResponse
{
    public long Id { get; init; }
    public string Action { get; init; } = default!;
    public Guid ActorId { get; init; }
    public string ActorName { get; init; } = default!;
    public Guid? TargetUserId { get; init; }
    public string? TargetUserName { get; init; }
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = default!;
    public string? PermissionCodes { get; init; }
    public string? Detail { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
