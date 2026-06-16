using Domain.Base;
using Domain.Enums.Roles;

namespace Domain.Entities.Roles;

public class Permission : BaseEntity, IAuditable
{
    public string PermissionCode { get; private set; } = null!;
    public string PermissionName { get; private set; } = null!;
    public PermissionModule Module { get; private set; }
    public PermissionAction Action { get; private set; }
    public string Resource { get; private set; } = null!;
    public string? Description { get; private set; }

    public bool IsActive { get; set; } = true;

    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = [];

    private Permission() : base()
    {
    }

    public static Permission Create(
        string permissionCode,
        string permissionName,
        PermissionModule module,
        PermissionAction action,
        string resource,
        string? description = null)
    {
        return new Permission
        {
            PermissionCode = permissionCode.ToLowerInvariant(),
            PermissionName = permissionName,
            Module = module,
            Action = action,
            Resource = resource.ToLowerInvariant(),
            Description = description,
            IsActive = true
        };
    }

    public void Update(string permissionName, string? description, bool isActive)
    {
        PermissionName = permissionName;
        Description = description;
        IsActive = isActive;
    }
}
