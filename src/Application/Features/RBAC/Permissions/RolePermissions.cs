namespace Application;

public static class RolePermissions
{
    [PermissionInfo("Xem danh sách vai trò", "Xem danh sách tất cả vai trò")]
    public const string ViewList = "rbac:roles:view-list";

    [PermissionInfo("Xem người dùng trong vai trò", "Xem danh sách nhân sự thuộc một vai trò")]
    public const string ViewUsers = "rbac:roles:view-users";

    [PermissionInfo("Tạo vai trò", "Tạo mới vai trò trong hệ thống")]
    public const string Create = "rbac:roles:create";

    [PermissionInfo("Cập nhật vai trò", "Chỉnh sửa thông tin vai trò")]
    public const string Update = "rbac:roles:update";

    [PermissionInfo("Xóa vai trò", "Xóa vai trò khỏi hệ thống")]
    public const string Delete = "rbac:roles:delete";

    [PermissionInfo("Gán quyền cho vai trò", "Thêm hoặc thu hồi quyền của một vai trò")]
    public const string AssignPermission = "rbac:roles:assign-permission";

    [PermissionInfo("Đồng bộ người dùng vào vai trò", "Cập nhật lại danh sách nhân sự thuộc vai trò")]
    public const string SyncUsers = "rbac:roles:sync-users";
}
