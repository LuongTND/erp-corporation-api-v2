namespace Domain;

public class PermissionAuditLog
{
    public long Id { get; set; }
    public string Action { get; set; } = default!;
    public Guid ActorId { get; set; }
    public string ActorName { get; set; } = default!;
    public Guid? TargetUserId { get; set; }
    public string? TargetUserName { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public string? PermissionCodes { get; set; }
    public string? Detail { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}
