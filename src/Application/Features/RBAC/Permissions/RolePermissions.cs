namespace Application;

public static class RolePermissions
{
    public const string ViewList = "rbac:roles:view-list";
    public const string ViewUsers = "rbac:roles:view-users";
    public const string Create = "rbac:roles:create";
    public const string Update = "rbac:roles:update";
    public const string Delete = "rbac:roles:delete";
    public const string AssignPermission = "rbac:roles:assign-permission";
    public const string SyncUsers = "rbac:roles:sync-users";
}
