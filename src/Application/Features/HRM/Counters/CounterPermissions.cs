namespace Application;

public static class CounterPermissions
{
    [PermissionInfo("Xem danh sách quầy hàng", "Xem toàn bộ danh sách quầy hàng")]
    public const string ViewList = "hrm:counters:view-list";

    [PermissionInfo("Tạo quầy hàng", "Tạo mới quầy hàng")]
    public const string Create = "hrm:counters:create";

    [PermissionInfo("Cập nhật quầy hàng", "Chỉnh sửa thông tin quầy hàng")]
    public const string Update = "hrm:counters:update";

    [PermissionInfo("Xóa quầy hàng", "Xóa quầy hàng")]
    public const string Delete = "hrm:counters:delete";
}
