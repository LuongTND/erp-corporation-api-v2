namespace Application;

public static class DepartmentManagerPermissions
{
    [PermissionInfo("Xem phòng ban phụ trách", "Xem danh sách phòng ban mà quản lý đang phụ trách")]
    public const string ViewMyDepartments = "hrm:department-manager:view-departments";
}
