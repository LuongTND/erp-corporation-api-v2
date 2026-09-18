namespace Application;

public static class DepartmentPermissions
{
    [PermissionInfo("Xem danh sách phòng ban", "Xem toàn bộ danh sách phòng ban")]
    public const string ViewList = "hrm:departments:view-list";

    [PermissionInfo("Xem chi tiết phòng ban", "Xem thông tin chi tiết một phòng ban")]
    public const string ViewDetail = "hrm:departments:view-detail";

    [PermissionInfo("Xem cây phòng ban", "Xem sơ đồ phân cấp phòng ban")]
    public const string ViewTree = "hrm:departments:view-tree";

    [PermissionInfo("Xem thành viên phòng ban", "Xem danh sách nhân sự trong phòng ban")]
    public const string ViewMembers = "hrm:departments:view-members";

    [PermissionInfo("Tạo phòng ban", "Tạo mới phòng ban")]
    public const string Create = "hrm:departments:create";

    [PermissionInfo("Cập nhật phòng ban", "Chỉnh sửa thông tin phòng ban")]
    public const string Update = "hrm:departments:update";

    [PermissionInfo("Xóa phòng ban", "Xóa phòng ban khỏi hệ thống")]
    public const string Delete = "hrm:departments:delete";
}
