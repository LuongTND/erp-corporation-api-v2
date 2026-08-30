namespace Application;

public static class UserPermissions
{
    [PermissionInfo("Tạo nhân sự", "Tạo hồ sơ nhân sự mới")]
    public const string Create = "hrm:users:create";

    [PermissionInfo("Xem hồ sơ nhân sự", "Xem thông tin chi tiết hồ sơ nhân sự")]
    public const string View = "hrm:users:view";

    [PermissionInfo("Xem lịch sử nhân sự", "Xem lịch sử thay đổi của hồ sơ nhân sự")]
    public const string ViewHistory = "hrm:users:view-history";

    [PermissionInfo("Xuất dữ liệu nhân sự", "Xuất danh sách nhân sự ra file")]
    public const string Export = "hrm:users:export";

    [PermissionInfo("Cập nhật hồ sơ nhân sự", "Chỉnh sửa thông tin cá nhân của nhân sự")]
    public const string UpdateProfile = "hrm:users:update-profile";

    [PermissionInfo("Cập nhật trường tùy chỉnh", "Chỉnh sửa các trường mở rộng trong hồ sơ nhân sự")]
    public const string UpdateCustomFields = "hrm:users:update-custom-fields";

    [PermissionInfo("Cập nhật trạng thái nhân sự", "Thay đổi trạng thái làm việc của nhân sự")]
    public const string UpdateStatus = "hrm:users:update-status";

    [PermissionInfo("Khóa tài khoản nhân sự", "Khóa hoặc mở khóa tài khoản nhân sự")]
    public const string Lock = "hrm:users:lock";

    [PermissionInfo("Gán loại nhân sự", "Gán loại hợp đồng / phân loại nhân sự")]
    public const string AssignEmployeeType = "hrm:users:assign-employee-type";

    [PermissionInfo("Xóa cấp bậc nhân sự", "Gỡ bỏ cấp bậc công việc của nhân sự")]
    public const string RemoveJobLevel = "hrm:users:remove-job-level";

    [PermissionInfo("Thêm nhân sự vào phòng ban", "Gán nhân sự vào một phòng ban")]
    public const string AddDepartment = "hrm:users:add-department";

    [PermissionInfo("Cập nhật phòng ban nhân sự", "Thay đổi thông tin phòng ban của nhân sự")]
    public const string UpdateDepartment = "hrm:users:update-department";

    [PermissionInfo("Xóa nhân sự khỏi phòng ban", "Gỡ nhân sự ra khỏi phòng ban")]
    public const string RemoveDepartment = "hrm:users:remove-department";

    [PermissionInfo("Chuyển phòng ban nhân sự", "Chuyển nhân sự sang phòng ban khác")]
    public const string TransferDepartment = "hrm:users:transfer-department";

    [PermissionInfo("Gán vai trò cho nhân sự", "Phân quyền vai trò cho nhân sự")]
    public const string AssignRole = "hrm:users:assign-role";

    [PermissionInfo("Thu hồi vai trò nhân sự", "Gỡ bỏ vai trò đã gán cho nhân sự")]
    public const string RevokeRole = "hrm:users:revoke-role";

    [PermissionInfo("Thiết lập phạm vi truy cập", "Xác định phạm vi dữ liệu nhân sự được phép thao tác")]
    public const string SetScope = "hrm:users:set-scope";
}
