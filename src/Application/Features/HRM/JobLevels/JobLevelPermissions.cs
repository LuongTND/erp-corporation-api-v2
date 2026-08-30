namespace Application;

public static class JobLevelPermissions
{
    [PermissionInfo("Xem danh sách cấp bậc", "Xem toàn bộ cấp bậc công việc")]
    public const string ViewList = "hrm:job-levels:view-list";

    [PermissionInfo("Xem chi tiết cấp bậc", "Xem thông tin chi tiết một cấp bậc")]
    public const string ViewDetail = "hrm:job-levels:view-detail";

    [PermissionInfo("Tạo cấp bậc", "Tạo mới cấp bậc công việc")]
    public const string Create = "hrm:job-levels:create";

    [PermissionInfo("Cập nhật cấp bậc", "Chỉnh sửa thông tin cấp bậc")]
    public const string Update = "hrm:job-levels:update";

    [PermissionInfo("Xóa cấp bậc", "Xóa cấp bậc công việc")]
    public const string Delete = "hrm:job-levels:delete";
}
