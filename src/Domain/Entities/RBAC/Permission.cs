namespace Domain;

public class Permission : EntityBase<Guid>
{
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public PermissionModule Module { get; set; }
    public PermissionAction Action { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
