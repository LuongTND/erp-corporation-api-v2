namespace Domain;

public class Role : AuditableEntityBase<Guid>
{
    public string RoleName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool BypassDataScope { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
