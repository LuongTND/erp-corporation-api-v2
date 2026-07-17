namespace Domain;

public class Role : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public string RoleName { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }
    public bool BypassDataScope { get; private set; }

    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = [];
    public virtual ICollection<UserRole> UserRoles { get; private set; } = [];

    private Role() : base()
    {
    }

    public static Role Create(
        string roleName,
        string displayName,
        string? description = null,
        bool isSystemRole = false,
        bool bypassDataScope = false)
    {
        return new Role
        {
            RoleName = roleName.ToUpperInvariant(),
            DisplayName = displayName,
            Description = description,
            IsSystemRole = isSystemRole,
            BypassDataScope = bypassDataScope,
            IsActive = true
        };
    }

    public void Update(
        string displayName,
        string? description = null,
        bool bypassDataScope = false)
    {
        DisplayName = displayName;
        Description = description;
        BypassDataScope = bypassDataScope;
    }
}