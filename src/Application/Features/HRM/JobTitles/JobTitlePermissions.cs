namespace Application;

public static class JobTitlePermissions
{
    [PermissionInfo("Xem danh sách cấp bậc", "Xem toàn bộ cấp bậc công việc")]
    public const string ViewList = "hrm:job-titles:view-list";

    [PermissionInfo("Xem chi tiết cấp bậc", "Xem thông tin chi tiết một cấp bậc")]
    public const string ViewDetail = "hrm:job-titles:view-detail";

    [PermissionInfo("Tạo cấp bậc", "Tạo mới cấp bậc công việc")]
    public const string Create = "hrm:job-titles:create";

    [PermissionInfo("Cập nhật cấp bậc", "Chỉnh sửa thông tin cấp bậc")]
    public const string Update = "hrm:job-titles:update";

    [PermissionInfo("Xóa cấp bậc", "Xóa cấp bậc công việc")]
    public const string Delete = "hrm:job-titles:delete";
}
