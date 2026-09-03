namespace Application;

public static class WorkflowPermissions
{
    [PermissionInfo("Xem task duyệt của tôi", "Xem danh sách các task đang chờ tôi duyệt")]
    public const string ViewMyTasks = "workflow:task:view";

    [PermissionInfo("Xem lịch sử duyệt", "Xem toàn bộ lịch sử duyệt của một instance")]
    public const string ViewInstanceTasks = "workflow:instance:view";

    [PermissionInfo("Quản lý workflow template", "Tạo, sửa, xóa workflow template và các bước duyệt")]
    public const string ManageTemplates = "workflow:template:manage";
}
