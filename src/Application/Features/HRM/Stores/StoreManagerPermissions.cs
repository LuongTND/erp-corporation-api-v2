namespace Application;

public static class StoreManagerPermissions
{
    [PermissionInfo("Xem thông tin cửa hàng phụ trách", "Xem chi tiết cửa hàng mà quản lý đang phụ trách")]
    public const string ViewMyStore = "hrm:store-manager:view-store";

    [PermissionInfo("Xem nhân viên cửa hàng phụ trách", "Xem danh sách nhân viên trong cửa hàng phụ trách")]
    public const string ViewMyStoreMembers = "hrm:store-manager:view-members";
}
