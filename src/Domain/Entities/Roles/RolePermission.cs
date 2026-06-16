namespace Domain.Entities.Roles;

public class RolePermission
{
    public Guid RoleId { get; private set; }
    public virtual Role Role { get; private set; } = null!;

    public Guid PermissionId { get; private set; }
    public virtual Permission Permission { get; private set; } = null!;

    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    private RolePermission()
    {
    }

    public static RolePermission Create(Guid roleId, Guid permissionId, Guid? assignedBy = null)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        };
    }
}
