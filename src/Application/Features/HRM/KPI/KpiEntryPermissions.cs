namespace Application;

public static class KpiEntryPermissions
{
    [PermissionInfo("Xem danh sách KPI", "Xem danh sách kết quả đánh giá KPI")]
    public const string ViewList = "hrm:kpi-entries:view-list";

    [PermissionInfo("Xem tổng hợp KPI", "Xem báo cáo tổng hợp kết quả KPI")]
    public const string ViewSummary = "hrm:kpi-entries:view-summary";

    [PermissionInfo("Nhập / cập nhật KPI", "Tạo mới hoặc cập nhật kết quả đánh giá KPI")]
    public const string Upsert = "hrm:kpi-entries:upsert";
}
