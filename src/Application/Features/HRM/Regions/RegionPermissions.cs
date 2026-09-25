namespace Application;

public static class RegionPermissions
{
    [PermissionInfo("Xem danh sách khu vực", "Xem toàn bộ danh sách khu vực")]
    public const string ViewList = "hrm:regions:view-list";

    [PermissionInfo("Đồng bộ khu vực", "Đồng bộ dữ liệu khu vực từ hệ thống ngoài")]
    public const string Sync = "hrm:regions:sync";

    [PermissionInfo("Xem giờ làm việc khu vực", "Xem lịch giờ hoạt động của khu vực")]
    public const string ViewHours = "hrm:regions:view-hours";

    [PermissionInfo("Cập nhật giờ làm việc khu vực", "Chỉnh sửa lịch giờ hoạt động khu vực")]
    public const string UpdateHours = "hrm:regions:update-hours";

    [PermissionInfo("Gán quản lý khu vực", "Gán hoặc gỡ người quản lý cho khu vực")]
    public const string AssignManager = "hrm:regions:assign-manager";
}
