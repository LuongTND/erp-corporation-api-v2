

namespace Infrastructure;
/// <summary>
/// Seed dữ liệu thông báo — idempotent, chạy mỗi lần khởi động dev.
/// </summary>
public static class NotificationInitialData
{
    public static class EventTypeIds
    {
        public static readonly Guid GenericInfo = Guid.Parse("a1000001-0000-4000-8000-000000000001");
        public static readonly Guid GenericSuccess = Guid.Parse("a1000001-0000-4000-8000-000000000002");
        public static readonly Guid GenericWarning = Guid.Parse("a1000001-0000-4000-8000-000000000003");
        public static readonly Guid GenericError = Guid.Parse("a1000001-0000-4000-8000-000000000004");

        public static readonly Guid UserCreated = Guid.Parse("a1000001-0000-4000-8000-000000000010");
        public static readonly Guid UserUpdated = Guid.Parse("a1000001-0000-4000-8000-000000000011");
        public static readonly Guid UserDeleted = Guid.Parse("a1000001-0000-4000-8000-000000000012");
        public static readonly Guid UserRolesAssigned = Guid.Parse("a1000001-0000-4000-8000-000000000013");
        public static readonly Guid UserPasswordReset = Guid.Parse("a1000001-0000-4000-8000-000000000014");
        public static readonly Guid UserSecondaryDeptAdded = Guid.Parse("a1000001-0000-4000-8000-000000000015");
        public static readonly Guid UserSecondaryDeptRemoved = Guid.Parse("a1000001-0000-4000-8000-000000000016");

        public static readonly Guid DepartmentCreated = Guid.Parse("a1000001-0000-4000-8000-000000000020");
        public static readonly Guid DepartmentUpdated = Guid.Parse("a1000001-0000-4000-8000-000000000021");
        public static readonly Guid DepartmentDeleted = Guid.Parse("a1000001-0000-4000-8000-000000000022");

        public static readonly Guid JobLevelCreated = Guid.Parse("a1000001-0000-4000-8000-000000000030");
        public static readonly Guid JobLevelUpdated = Guid.Parse("a1000001-0000-4000-8000-000000000031");
        public static readonly Guid JobLevelDeleted = Guid.Parse("a1000001-0000-4000-8000-000000000032");

        public static readonly Guid RoleCreated = Guid.Parse("a1000001-0000-4000-8000-000000000040");
        public static readonly Guid RoleUpdated = Guid.Parse("a1000001-0000-4000-8000-000000000041");
        public static readonly Guid RoleDeleted = Guid.Parse("a1000001-0000-4000-8000-000000000042");
        public static readonly Guid RolePermissionsUpdated = Guid.Parse("a1000001-0000-4000-8000-000000000043");

        public static readonly Guid PermissionCreated = Guid.Parse("a1000001-0000-4000-8000-000000000050");
        public static readonly Guid PermissionUpdated = Guid.Parse("a1000001-0000-4000-8000-000000000051");
        public static readonly Guid PermissionDeleted = Guid.Parse("a1000001-0000-4000-8000-000000000052");
    }

    public static class TriggerIds
    {
        public static readonly Guid UserCreate = Guid.Parse("b2000001-0000-4000-8000-000000000001");
        public static readonly Guid UserUpdate = Guid.Parse("b2000001-0000-4000-8000-000000000002");
        public static readonly Guid UserDelete = Guid.Parse("b2000001-0000-4000-8000-000000000003");
        public static readonly Guid UserAssignRoles = Guid.Parse("b2000001-0000-4000-8000-000000000004");
        public static readonly Guid UserResetPassword = Guid.Parse("b2000001-0000-4000-8000-000000000005");
        public static readonly Guid UserSecondaryDeptAdd = Guid.Parse("b2000001-0000-4000-8000-000000000006");
        public static readonly Guid UserSecondaryDeptRemove = Guid.Parse("b2000001-0000-4000-8000-000000000007");

        public static readonly Guid DepartmentCreate = Guid.Parse("b2000001-0000-4000-8000-000000000008");
        public static readonly Guid DepartmentUpdate = Guid.Parse("b2000001-0000-4000-8000-000000000009");
        public static readonly Guid DepartmentDelete = Guid.Parse("b2000001-0000-4000-8000-000000000010");

        public static readonly Guid JobLevelCreate = Guid.Parse("b2000001-0000-4000-8000-000000000011");
        public static readonly Guid JobLevelUpdate = Guid.Parse("b2000001-0000-4000-8000-000000000012");
        public static readonly Guid JobLevelDelete = Guid.Parse("b2000001-0000-4000-8000-000000000013");

        public static readonly Guid RoleCreate = Guid.Parse("b2000001-0000-4000-8000-000000000014");
        public static readonly Guid RoleUpdate = Guid.Parse("b2000001-0000-4000-8000-000000000015");
        public static readonly Guid RoleDelete = Guid.Parse("b2000001-0000-4000-8000-000000000016");
        public static readonly Guid RolePermissionsUpdate = Guid.Parse("b2000001-0000-4000-8000-000000000017");

        public static readonly Guid PermissionCreate = Guid.Parse("b2000001-0000-4000-8000-000000000018");
        public static readonly Guid PermissionUpdate = Guid.Parse("b2000001-0000-4000-8000-000000000019");
        public static readonly Guid PermissionDelete = Guid.Parse("b2000001-0000-4000-8000-000000000020");
    }

    public record EventTypeSeed(
        Guid Id,
        string EventCode,
        string Name,
        string Module,
        string DefaultTitleTemplate,
        string DefaultBodyTemplate,
        string? Description);

    public record TriggerSeed(
        Guid Id,
        string TriggerKey,
        string Name,
        string Module,
        Guid? EventTypeId,
        string? LinkUrlTemplate,
        string? Description,
        string? RecipientRulesJson = null);

    public static readonly string DefaultUserEventRecipientRulesJson =
        NotificationRecipientRulesJson.Serialize(NotificationRecipientRulesDefaults.ForUserEvents());

    public static readonly string DefaultAdminEventRecipientRulesJson =
        NotificationRecipientRulesJson.Serialize(NotificationRecipientRulesDefaults.ForAdminEvents());

    public static readonly EventTypeSeed[] EventTypes =
    [
        new(
            EventTypeIds.GenericInfo,
            "system.generic.info",
            "Thông tin chung",
            "System",
            "{{title}}",
            "{{message}}",
            "Placeholder: {{title}}, {{message}}, {{actorName}}"),
        new(
            EventTypeIds.GenericSuccess,
            "system.generic.success",
            "Thông báo thành công",
            "System",
            "{{title}}",
            "{{message}}",
            "Placeholder: {{title}}, {{message}}, {{actorName}}"),
        new(
            EventTypeIds.GenericWarning,
            "system.generic.warning",
            "Cảnh báo",
            "System",
            "{{title}}",
            "{{message}}",
            "Placeholder: {{title}}, {{message}}"),
        new(
            EventTypeIds.GenericError,
            "system.generic.error",
            "Lỗi / từ chối",
            "System",
            "{{title}}",
            "{{message}}",
            "Placeholder: {{title}}, {{message}}"),

        new(
            EventTypeIds.UserCreated,
            "hrm.user.created",
            "Nhân viên mới",
            "HRM",
            "Chào mừng {{fullName}}",
            "Tài khoản nhân viên {{employeeCode}} đã được tạo bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{employeeCode}}, {{actorName}}, {{userId}}"),
        new(
            EventTypeIds.UserUpdated,
            "hrm.user.updated",
            "Cập nhật nhân viên",
            "HRM",
            "Thông tin đã cập nhật",
            "Hồ sơ {{fullName}} ({{employeeCode}}) đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{employeeCode}}, {{actorName}}, {{userId}}"),
        new(
            EventTypeIds.UserDeleted,
            "hrm.user.deleted",
            "Nghỉ việc / vô hiệu hóa nhân viên",
            "HRM",
            "Nhân viên đã nghỉ việc",
            "Hồ sơ {{fullName}} ({{employeeCode}}) đã được vô hiệu hóa bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{employeeCode}}, {{actorName}}, {{userId}}"),
        new(
            EventTypeIds.UserRolesAssigned,
            "hrm.user.roles-assigned",
            "Gán vai trò nhân viên",
            "HRM",
            "Vai trò đã được cập nhật",
            "Vai trò của {{fullName}} ({{employeeCode}}) đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{employeeCode}}, {{actorName}}, {{userId}}"),
        new(
            EventTypeIds.UserPasswordReset,
            "hrm.user.password-reset",
            "Đặt lại mật khẩu",
            "HRM",
            "Mật khẩu đã được đặt lại",
            "Mật khẩu tài khoản {{fullName}} ({{employeeCode}}) đã được đặt lại bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{employeeCode}}, {{actorName}}, {{userId}}"),
        new(
            EventTypeIds.UserSecondaryDeptAdded,
            "hrm.user.secondary-dept-added",
            "Thêm phòng ban kiêm nhiệm",
            "HRM",
            "Phòng ban kiêm nhiệm mới",
            "{{fullName}} được gán kiêm nhiệm phòng {{departmentName}} bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{departmentName}}, {{actorName}}, {{userId}}, {{departmentId}}"),
        new(
            EventTypeIds.UserSecondaryDeptRemoved,
            "hrm.user.secondary-dept-removed",
            "Gỡ phòng ban kiêm nhiệm",
            "HRM",
            "Kết thúc kiêm nhiệm",
            "{{fullName}} đã kết thúc kiêm nhiệm phòng {{departmentName}} bởi {{actorName}}.",
            "Placeholder: {{fullName}}, {{departmentName}}, {{actorName}}, {{userId}}, {{departmentId}}"),

        new(
            EventTypeIds.DepartmentCreated,
            "hrm.department.created",
            "Phòng ban mới",
            "HRM",
            "Phòng ban mới: {{departmentName}}",
            "Phòng ban {{departmentName}} ({{departmentCode}}) đã được tạo bởi {{actorName}}.",
            "Placeholder: {{departmentName}}, {{departmentCode}}, {{actorName}}, {{departmentId}}"),
        new(
            EventTypeIds.DepartmentUpdated,
            "hrm.department.updated",
            "Cập nhật phòng ban",
            "HRM",
            "Phòng ban đã cập nhật",
            "Phòng ban {{departmentName}} ({{departmentCode}}) đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{departmentName}}, {{departmentCode}}, {{actorName}}, {{departmentId}}"),
        new(
            EventTypeIds.DepartmentDeleted,
            "hrm.department.deleted",
            "Vô hiệu hóa phòng ban",
            "HRM",
            "Phòng ban đã vô hiệu hóa",
            "Phòng ban {{departmentName}} ({{departmentCode}}) đã được vô hiệu hóa bởi {{actorName}}.",
            "Placeholder: {{departmentName}}, {{departmentCode}}, {{actorName}}, {{departmentId}}"),

        new(
            EventTypeIds.JobLevelCreated,
            "hrm.joblevel.created",
            "Cấp bậc mới",
            "HRM",
            "Cấp bậc mới: {{levelName}}",
            "Cấp bậc {{levelName}} đã được tạo bởi {{actorName}}.",
            "Placeholder: {{levelName}}, {{actorName}}, {{jobLevelId}}"),
        new(
            EventTypeIds.JobLevelUpdated,
            "hrm.joblevel.updated",
            "Cập nhật cấp bậc",
            "HRM",
            "Cấp bậc đã cập nhật",
            "Cấp bậc {{levelName}} đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{levelName}}, {{actorName}}, {{jobLevelId}}"),
        new(
            EventTypeIds.JobLevelDeleted,
            "hrm.joblevel.deleted",
            "Vô hiệu hóa cấp bậc",
            "HRM",
            "Cấp bậc đã vô hiệu hóa",
            "Cấp bậc {{levelName}} đã được vô hiệu hóa bởi {{actorName}}.",
            "Placeholder: {{levelName}}, {{actorName}}, {{jobLevelId}}"),

        new(
            EventTypeIds.RoleCreated,
            "system.role.created",
            "Vai trò mới",
            "System",
            "Vai trò mới: {{displayName}}",
            "Vai trò {{displayName}} ({{roleName}}) đã được tạo bởi {{actorName}}.",
            "Placeholder: {{displayName}}, {{roleName}}, {{actorName}}, {{roleId}}"),
        new(
            EventTypeIds.RoleUpdated,
            "system.role.updated",
            "Cập nhật vai trò",
            "System",
            "Vai trò đã cập nhật",
            "Vai trò {{displayName}} ({{roleName}}) đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{displayName}}, {{roleName}}, {{actorName}}, {{roleId}}"),
        new(
            EventTypeIds.RoleDeleted,
            "system.role.deleted",
            "Vô hiệu hóa vai trò",
            "System",
            "Vai trò đã vô hiệu hóa",
            "Vai trò {{displayName}} ({{roleName}}) đã được vô hiệu hóa bởi {{actorName}}.",
            "Placeholder: {{displayName}}, {{roleName}}, {{actorName}}, {{roleId}}"),
        new(
            EventTypeIds.RolePermissionsUpdated,
            "system.role.permissions-updated",
            "Cập nhật quyền vai trò",
            "System",
            "Quyền vai trò đã thay đổi",
            "Quyền của vai trò {{displayName}} ({{roleName}}) đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{displayName}}, {{roleName}}, {{actorName}}, {{roleId}}"),

        new(
            EventTypeIds.PermissionCreated,
            "system.permission.created",
            "Quyền mới",
            "System",
            "Quyền mới: {{permissionName}}",
            "Quyền {{permissionName}} ({{permissionCode}}) đã được tạo bởi {{actorName}}.",
            "Placeholder: {{permissionName}}, {{permissionCode}}, {{actorName}}, {{permissionId}}"),
        new(
            EventTypeIds.PermissionUpdated,
            "system.permission.updated",
            "Cập nhật quyền",
            "System",
            "Quyền đã cập nhật",
            "Quyền {{permissionName}} ({{permissionCode}}) đã được cập nhật bởi {{actorName}}.",
            "Placeholder: {{permissionName}}, {{permissionCode}}, {{actorName}}, {{permissionId}}"),
        new(
            EventTypeIds.PermissionDeleted,
            "system.permission.deleted",
            "Vô hiệu hóa quyền",
            "System",
            "Quyền đã vô hiệu hóa",
            "Quyền {{permissionName}} ({{permissionCode}}) đã được vô hiệu hóa bởi {{actorName}}.",
            "Placeholder: {{permissionName}}, {{permissionCode}}, {{actorName}}, {{permissionId}}"),
    ];

    public static readonly TriggerSeed[] Triggers =
    [
        new(
            TriggerIds.UserCreate,
            NotificationTriggers.UserCreate,
            "Tạo nhân viên",
            "HRM",
            EventTypeIds.UserCreated,
            "/users/{{userId}}",
            "Kích hoạt sau khi tạo nhân viên thành công.",
            DefaultUserEventRecipientRulesJson),
        new(
            TriggerIds.UserUpdate,
            NotificationTriggers.UserUpdate,
            "Cập nhật nhân viên",
            "HRM",
            EventTypeIds.UserUpdated,
            "/users/{{userId}}",
            "Kích hoạt sau khi cập nhật hồ sơ nhân viên.",
            DefaultUserEventRecipientRulesJson),
        new(
            TriggerIds.UserDelete,
            NotificationTriggers.UserDelete,
            "Vô hiệu hóa nhân viên",
            "HRM",
            EventTypeIds.UserDeleted,
            "/users/{{userId}}",
            "Kích hoạt sau khi vô hiệu hóa nhân viên.",
            DefaultUserEventRecipientRulesJson),
        new(
            TriggerIds.UserAssignRoles,
            NotificationTriggers.UserAssignRoles,
            "Gán vai trò nhân viên",
            "HRM",
            EventTypeIds.UserRolesAssigned,
            "/users/{{userId}}",
            "Kích hoạt sau khi cập nhật vai trò của nhân viên.",
            DefaultUserEventRecipientRulesJson),
        new(
            TriggerIds.UserResetPassword,
            NotificationTriggers.UserResetPassword,
            "Đặt lại mật khẩu",
            "HRM",
            EventTypeIds.UserPasswordReset,
            "/users/{{userId}}",
            "Kích hoạt sau khi admin đặt lại mật khẩu nhân viên.",
            DefaultUserEventRecipientRulesJson),
        new(
            TriggerIds.UserSecondaryDeptAdd,
            NotificationTriggers.UserSecondaryDeptAdd,
            "Thêm phòng ban kiêm nhiệm",
            "HRM",
            EventTypeIds.UserSecondaryDeptAdded,
            "/users/{{userId}}",
            "Kích hoạt sau khi gán phòng ban kiêm nhiệm.",
            DefaultUserEventRecipientRulesJson),
        new(
            TriggerIds.UserSecondaryDeptRemove,
            NotificationTriggers.UserSecondaryDeptRemove,
            "Gỡ phòng ban kiêm nhiệm",
            "HRM",
            EventTypeIds.UserSecondaryDeptRemoved,
            "/users/{{userId}}",
            "Kích hoạt sau khi kết thúc kiêm nhiệm phòng ban.",
            DefaultUserEventRecipientRulesJson),

        new(
            TriggerIds.DepartmentCreate,
            NotificationTriggers.DepartmentCreate,
            "Tạo phòng ban",
            "HRM",
            EventTypeIds.DepartmentCreated,
            "/departments/{{departmentId}}",
            "Kích hoạt sau khi tạo phòng ban.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.DepartmentUpdate,
            NotificationTriggers.DepartmentUpdate,
            "Cập nhật phòng ban",
            "HRM",
            EventTypeIds.DepartmentUpdated,
            "/departments/{{departmentId}}",
            "Kích hoạt sau khi cập nhật phòng ban.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.DepartmentDelete,
            NotificationTriggers.DepartmentDelete,
            "Vô hiệu hóa phòng ban",
            "HRM",
            EventTypeIds.DepartmentDeleted,
            "/departments/{{departmentId}}",
            "Kích hoạt sau khi vô hiệu hóa phòng ban.",
            DefaultAdminEventRecipientRulesJson),

        new(
            TriggerIds.JobLevelCreate,
            NotificationTriggers.JobLevelCreate,
            "Tạo cấp bậc",
            "HRM",
            EventTypeIds.JobLevelCreated,
            "/job-levels/{{jobLevelId}}",
            "Kích hoạt sau khi tạo cấp bậc chức danh.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.JobLevelUpdate,
            NotificationTriggers.JobLevelUpdate,
            "Cập nhật cấp bậc",
            "HRM",
            EventTypeIds.JobLevelUpdated,
            "/job-levels/{{jobLevelId}}",
            "Kích hoạt sau khi cập nhật cấp bậc chức danh.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.JobLevelDelete,
            NotificationTriggers.JobLevelDelete,
            "Vô hiệu hóa cấp bậc",
            "HRM",
            EventTypeIds.JobLevelDeleted,
            "/job-levels/{{jobLevelId}}",
            "Kích hoạt sau khi vô hiệu hóa cấp bậc chức danh.",
            DefaultAdminEventRecipientRulesJson),

        new(
            TriggerIds.RoleCreate,
            NotificationTriggers.RoleCreate,
            "Tạo vai trò",
            "System",
            EventTypeIds.RoleCreated,
            "/roles/{{roleId}}",
            "Kích hoạt sau khi tạo vai trò mới.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.RoleUpdate,
            NotificationTriggers.RoleUpdate,
            "Cập nhật vai trò",
            "System",
            EventTypeIds.RoleUpdated,
            "/roles/{{roleId}}",
            "Kích hoạt sau khi cập nhật thông tin vai trò.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.RoleDelete,
            NotificationTriggers.RoleDelete,
            "Vô hiệu hóa vai trò",
            "System",
            EventTypeIds.RoleDeleted,
            "/roles/{{roleId}}",
            "Kích hoạt sau khi vô hiệu hóa vai trò.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.RolePermissionsUpdate,
            NotificationTriggers.RolePermissionsUpdate,
            "Cập nhật quyền vai trò",
            "System",
            EventTypeIds.RolePermissionsUpdated,
            "/roles/{{roleId}}",
            "Kích hoạt sau khi thay đổi danh sách quyền của vai trò.",
            DefaultAdminEventRecipientRulesJson),

        new(
            TriggerIds.PermissionCreate,
            NotificationTriggers.PermissionCreate,
            "Tạo quyền",
            "System",
            EventTypeIds.PermissionCreated,
            "/permissions/{{permissionId}}",
            "Kích hoạt sau khi tạo quyền mới.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.PermissionUpdate,
            NotificationTriggers.PermissionUpdate,
            "Cập nhật quyền",
            "System",
            EventTypeIds.PermissionUpdated,
            "/permissions/{{permissionId}}",
            "Kích hoạt sau khi cập nhật quyền.",
            DefaultAdminEventRecipientRulesJson),
        new(
            TriggerIds.PermissionDelete,
            NotificationTriggers.PermissionDelete,
            "Vô hiệu hóa quyền",
            "System",
            EventTypeIds.PermissionDeleted,
            "/permissions/{{permissionId}}",
            "Kích hoạt sau khi vô hiệu hóa quyền.",
            DefaultAdminEventRecipientRulesJson),
    ];

    public static readonly InitialData.PermissionSeed[] NotificationPermissions =
    [
        new("system.notification.event.read", "Xem loại thông báo", PermissionModule.System, PermissionAction.Read, "notification-event"),
        new("system.notification.event.create", "Tạo loại thông báo", PermissionModule.System, PermissionAction.Create, "notification-event"),
        new("system.notification.event.update", "Cập nhật loại thông báo", PermissionModule.System, PermissionAction.Update, "notification-event"),
        new("system.notification.event.delete", "Vô hiệu hóa loại thông báo", PermissionModule.System, PermissionAction.Delete, "notification-event"),
        new("system.notification.trigger.read", "Xem gán thông báo chức năng", PermissionModule.System, PermissionAction.Read, "notification-trigger"),
        new("system.notification.trigger.update", "Gán thông báo cho chức năng", PermissionModule.System, PermissionAction.Update, "notification-trigger"),
    ];
}
