namespace Application;

public static class KpiTemplatePermissions
{
    [PermissionInfo("Xem danh sách mẫu KPI", "Xem toàn bộ mẫu đánh giá KPI")]
    public const string ViewList = "hrm:kpi-templates:view-list";

    [PermissionInfo("Xem chi tiết mẫu KPI", "Xem thông tin chi tiết một mẫu KPI")]
    public const string ViewDetail = "hrm:kpi-templates:view-detail";

    [PermissionInfo("Tạo mẫu KPI", "Tạo mới mẫu đánh giá KPI")]
    public const string Create = "hrm:kpi-templates:create";

    [PermissionInfo("Cập nhật mẫu KPI", "Chỉnh sửa mẫu KPI")]
    public const string Update = "hrm:kpi-templates:update";

    [PermissionInfo("Xóa mẫu KPI", "Xóa mẫu KPI")]
    public const string Delete = "hrm:kpi-templates:delete";
}
