namespace Application;

public static class EmployeeTypePermissions
{
    [PermissionInfo("Xem loại nhân sự", "Xem danh sách các loại hợp đồng / phân loại nhân sự")]
    public const string ViewList = "hrm:employee-types:view-list";

    [PermissionInfo("Tạo loại nhân sự", "Tạo mới loại nhân sự")]
    public const string Create = "hrm:employee-types:create";

    [PermissionInfo("Cập nhật loại nhân sự", "Chỉnh sửa loại nhân sự")]
    public const string Update = "hrm:employee-types:update";

    [PermissionInfo("Xóa loại nhân sự", "Xóa loại nhân sự")]
    public const string Delete = "hrm:employee-types:delete";
}
