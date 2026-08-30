namespace Application;

public static class PayrollRunPermissions
{
    [PermissionInfo("Xem danh sách bảng lương", "Xem toàn bộ các kỳ bảng lương")]
    public const string ViewList = "hrm:payroll-runs:view-list";

    [PermissionInfo("Xem chi tiết bảng lương", "Xem thông tin chi tiết một kỳ bảng lương")]
    public const string ViewDetail = "hrm:payroll-runs:view-detail";

    [PermissionInfo("Tạo bảng lương", "Khởi tạo kỳ tính lương mới")]
    public const string Create = "hrm:payroll-runs:create";

    [PermissionInfo("Cập nhật dòng lương", "Chỉnh sửa thông tin lương từng nhân sự trong kỳ")]
    public const string UpdateEntry = "hrm:payroll-runs:update-entry";

    [PermissionInfo("Chốt bảng lương", "Xác nhận và khóa kỳ bảng lương")]
    public const string Finalize = "hrm:payroll-runs:finalize";
}
