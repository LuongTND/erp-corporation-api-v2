namespace Domain;

public class UserRole : BaseEntity, IAuditable
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public virtual Role Role { get; private set; } = null!;

    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    public DateTime? RevokedAt { get; private set; }
    public Guid? RevokedBy { get; private set; }

    public bool IsActive { get; set; } = true;

    private UserRole() : base()
    {
    }

    public static UserRole Create(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy,
            IsActive = true
        };
    }

    public void Revoke(Guid? revokedBy = null)
    {
        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy;
        IsActive = false;
    }
}