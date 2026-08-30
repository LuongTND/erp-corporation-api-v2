namespace Application;

public static class DepartmentJobLevelPermissions
{
    [PermissionInfo("Xem cấp bậc phòng ban", "Xem danh sách cấp bậc trong phòng ban")]
    public const string ViewList = "hrm:department-job-levels:view-list";

    [PermissionInfo("Xem chi tiết cấp bậc phòng ban", "Xem thông tin chi tiết cấp bậc")]
    public const string ViewDetail = "hrm:department-job-levels:view-detail";

    [PermissionInfo("Tạo cấp bậc phòng ban", "Tạo mới cấp bậc cho phòng ban")]
    public const string Create = "hrm:department-job-levels:create";

    [PermissionInfo("Cập nhật cấp bậc phòng ban", "Chỉnh sửa thông tin cấp bậc")]
    public const string Update = "hrm:department-job-levels:update";

    [PermissionInfo("Xóa cấp bậc phòng ban", "Xóa cấp bậc khỏi phòng ban")]
    public const string Delete = "hrm:department-job-levels:delete";

    [PermissionInfo("Gán mẫu KPI cho cấp bậc", "Liên kết mẫu KPI với cấp bậc trong phòng ban")]
    public const string AssignKpiTemplate = "hrm:department-job-levels:assign-kpi-template";
}
