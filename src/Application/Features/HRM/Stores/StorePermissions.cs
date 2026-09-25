namespace Application;

public static class StorePermissions
{
    [PermissionInfo("Xem danh sách cửa hàng", "Xem toàn bộ danh sách cửa hàng")]
    public const string ViewList = "hrm:stores:view-list";

    [PermissionInfo("Xóa cửa hàng", "Xóa cửa hàng khỏi hệ thống")]
    public const string Delete = "hrm:stores:delete";

    [PermissionInfo("Đồng bộ cửa hàng", "Đồng bộ dữ liệu cửa hàng từ hệ thống ngoài")]
    public const string Sync = "hrm:stores:sync";

    [PermissionInfo("Xem giờ làm việc cửa hàng", "Xem lịch giờ hoạt động của cửa hàng")]
    public const string ViewHours = "hrm:stores:view-hours";

    [PermissionInfo("Bật/tắt cửa hàng", "Kích hoạt hoặc vô hiệu hóa cửa hàng")]
    public const string ToggleActive = "hrm:stores:toggle-active";

    [PermissionInfo("Cập nhật giờ làm việc cửa hàng", "Chỉnh sửa lịch giờ hoạt động cửa hàng")]
    public const string UpdateHours = "hrm:stores:update-hours";

    [PermissionInfo("Gán quản lý cửa hàng", "Chỉ định quản lý cho cửa hàng")]
    public const string AssignManager = "hrm:stores:assign-manager";

    [PermissionInfo("Xem nhân viên cửa hàng", "Xem danh sách nhân viên trong cửa hàng")]
    public const string ViewMembers = "hrm:stores:view-members";

    [PermissionInfo("Thêm nhân viên vào cửa hàng", "Gán nhân viên vào cửa hàng")]
    public const string AddMember = "hrm:stores:add-member";

    [PermissionInfo("Xóa nhân viên khỏi cửa hàng", "Gỡ nhân viên ra khỏi cửa hàng")]
    public const string RemoveMember = "hrm:stores:remove-member";

    [PermissionInfo("Nhập cửa hàng từ POS", "Import dữ liệu cửa hàng từ hệ thống POS")]
    public const string ImportFromPos = "hrm:stores:import-from-pos";
}
