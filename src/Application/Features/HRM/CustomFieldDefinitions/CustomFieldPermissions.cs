namespace Application;

public static class CustomFieldPermissions
{
    [PermissionInfo("Xem danh sách trường tùy chỉnh", "Xem toàn bộ trường mở rộng trong hệ thống")]
    public const string ViewList = "hrm:custom-fields:view-list";

    [PermissionInfo("Xem chi tiết trường tùy chỉnh", "Xem thông tin chi tiết một trường tùy chỉnh")]
    public const string ViewDetail = "hrm:custom-fields:view-detail";

    [PermissionInfo("Tạo trường tùy chỉnh", "Tạo mới trường mở rộng")]
    public const string Create = "hrm:custom-fields:create";

    [PermissionInfo("Cập nhật trường tùy chỉnh", "Chỉnh sửa trường mở rộng")]
    public const string Update = "hrm:custom-fields:update";

    [PermissionInfo("Xóa trường tùy chỉnh", "Xóa trường mở rộng")]
    public const string Delete = "hrm:custom-fields:delete";
}
