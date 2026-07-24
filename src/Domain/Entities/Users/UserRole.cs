namespace Domain;

public class UserRole : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public DateTimeOffset AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? RevokedBy { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsValid() =>
        IsActive && RevokedAt == null && (ExpiresAt == null || ExpiresAt > DateTimeOffset.UtcNow);
}
