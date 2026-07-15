namespace Application;

public static class NotificationTriggers
{
    // HRM — Nhân sự
    public const string UserCreate = "hrm.user.create";
    public const string UserUpdate = "hrm.user.update";
    public const string UserDelete = "hrm.user.delete";
    public const string UserAssignRoles = "hrm.user.assign-roles";
    public const string UserResetPassword = "hrm.user.reset-password";
    public const string UserSecondaryDeptAdd = "hrm.user.secondary-dept.add";
    public const string UserSecondaryDeptRemove = "hrm.user.secondary-dept.remove";

    // HRM — Phòng ban
    public const string DepartmentCreate = "hrm.department.create";
    public const string DepartmentUpdate = "hrm.department.update";
    public const string DepartmentDelete = "hrm.department.delete";

    // HRM — Cấp bậc
    public const string JobLevelCreate = "hrm.joblevel.create";
    public const string JobLevelUpdate = "hrm.joblevel.update";
    public const string JobLevelDelete = "hrm.joblevel.delete";

    // System — Vai trò
    public const string RoleCreate = "system.role.create";
    public const string RoleUpdate = "system.role.update";
    public const string RoleDelete = "system.role.delete";
    public const string RolePermissionsUpdate = "system.role.permissions.update";

    // System — Quyền
    public const string PermissionCreate = "system.permission.create";
    public const string PermissionUpdate = "system.permission.update";
    public const string PermissionDelete = "system.permission.delete";
}