namespace Application;

public static class PermissionPermissions
{
    [PermissionInfo("Xem danh sách quyền", "Xem toàn bộ quyền hiện có trong hệ thống")]
    public const string ViewList = "rbac:permissions:view-list";

    [PermissionInfo("Xóa quyền", "Xóa quyền khỏi hệ thống")]
    public const string Delete = "rbac:permissions:delete";
}
