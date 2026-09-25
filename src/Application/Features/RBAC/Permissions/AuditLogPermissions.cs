namespace Application;

public static class AuditLogPermissions
{
    [PermissionInfo("Xem nhật ký thao tác", "Xem danh sách toàn bộ nhật ký hoạt động hệ thống")]
    public const string ViewList = "rbac:audit-logs:view-list";
}
