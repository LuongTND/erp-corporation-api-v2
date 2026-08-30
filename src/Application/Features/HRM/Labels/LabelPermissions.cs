namespace Application;

public static class LabelPermissions
{
    [PermissionInfo("Xem nhãn hồ sơ", "Xem danh sách nhãn và nhãn gắn trên hồ sơ nhân sự")]
    public const string View = "hrm:labels:view";

    [PermissionInfo("Quản lý nhãn", "Tạo, sửa, xóa nhãn hồ sơ nhân sự")]
    public const string Manage = "hrm:labels:manage";

    [PermissionInfo("Gán nhãn nhân sự", "Gán hoặc gỡ nhãn khỏi hồ sơ nhân sự")]
    public const string Assign = "hrm:labels:assign";
}
