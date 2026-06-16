
namespace Infrastructure.Persistence.Seed;

/// <summary>
/// Dữ liệu khởi tạo hệ thống — chỉnh sửa tại đây khi cần bổ sung seed mới.
/// DbInitializer đọc file này và ghi vào database khi setup lần đầu (DB chưa có nhân sự).
/// </summary>
public static class InitialData
{
    public static class Keys
    {
        public const string DefaultPassword = "!Abc123";

        public const string RoleSuperAdmin = "ROLE_SUPER_ADMIN";
        public const string RoleHrAdmin = "ROLE_HR_ADMIN";
        public const string RoleEmployee = "ROLE_EMPLOYEE";

        public const string DeptBoard = "BGĐ";
        public const string DeptHr = "HR";
        public const string DeptIt = "IT";
        public const string DeptAccounting = "KT";
        public const string DeptSales = "KD";

        public const string JobDirector = "Director";
        public const string JobManager = "Manager";
        public const string JobStaff = "Staff";
    }

    public record JobLevelSeed(
        string LevelName,
        int LevelOrder,
        ScopeType DefaultScopeType,
        string? Description,
        decimal? BaseSalaryMin = null,
        decimal? BaseSalaryMax = null);

    public record DepartmentSeed(
        string DepartmentName,
        string DepartmentCode,
        string? ParentDepartmentCode,
        string? Description);

    public record PermissionSeed(
        string PermissionCode,
        string PermissionName,
        PermissionModule Module,
        PermissionAction Action,
        string Resource);

    public record RoleSeed(
        string RoleName,
        string DisplayName,
        string? Description,
        bool IsSystemRole,
        bool BypassDataScope);

    public record UserSeed(
        string EmployeeCode,
        string FullName,
        string Email,
        string Password,
        string DepartmentCode,
        string JobLevelName,
        DateOnly DateOfJoin,
        UserStatus Status,
        string[] RoleNames,
        string? ManageDepartmentCode = null);

    public static readonly JobLevelSeed[] JobLevels =
    [
        new(Keys.JobDirector, 1, ScopeType.All, "Cấp giám đốc — truy cập toàn bộ dữ liệu", 50_000_000, 200_000_000),
        new(Keys.JobManager, 2, ScopeType.Department, "Cấp quản lý — phạm vi theo phòng ban", 25_000_000, 80_000_000),
        new(Keys.JobStaff, 3, ScopeType.Own, "Cấp nhân viên — chỉ dữ liệu của bản thân", 10_000_000, 35_000_000),
    ];

    public static readonly DepartmentSeed[] Departments =
    [
        new("Ban Giám Đốc", Keys.DeptBoard, null, "Hội đồng quản trị và Ban giám đốc"),
        new("Phòng Nhân Sự", Keys.DeptHr, Keys.DeptBoard, "Quản lý nhân sự, tuyển dụng và tiền lương"),
        new("Phòng Công Nghệ", Keys.DeptIt, Keys.DeptBoard, "Phát triển và vận hành hệ thống CNTT"),
        new("Phòng Kế Toán", Keys.DeptAccounting, Keys.DeptBoard, "Kế toán, tài chính và báo cáo"),
        new("Phòng Kinh Doanh", Keys.DeptSales, Keys.DeptBoard, "Bán hàng và chăm sóc khách hàng"),
    ];

    public static readonly PermissionSeed[] Permissions =
    [
        // Nhân sự
        new("hrm.employee.read", "Xem danh sách nhân viên", PermissionModule.Hrm, PermissionAction.Read, "employee"),
        new("hrm.employee.create", "Thêm mới nhân viên", PermissionModule.Hrm, PermissionAction.Create, "employee"),
        new("hrm.employee.update", "Sửa thông tin nhân viên", PermissionModule.Hrm, PermissionAction.Update, "employee"),
        new("hrm.employee.delete", "Xóa/Vô hiệu hóa nhân viên", PermissionModule.Hrm, PermissionAction.Delete, "employee"),

        // Phòng ban
        new("hrm.department.read", "Xem danh sách phòng ban", PermissionModule.Hrm, PermissionAction.Read, "department"),
        new("hrm.department.create", "Thêm mới phòng ban", PermissionModule.Hrm, PermissionAction.Create, "department"),
        new("hrm.department.update", "Sửa thông tin phòng ban", PermissionModule.Hrm, PermissionAction.Update, "department"),
        new("hrm.department.delete", "Xóa phòng ban", PermissionModule.Hrm, PermissionAction.Delete, "department"),

        // Cấp bậc
        new("hrm.joblevel.read", "Xem danh sách cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Read, "joblevel"),
        new("hrm.joblevel.create", "Thêm mới cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Create, "joblevel"),
        new("hrm.joblevel.update", "Sửa thông tin cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Update, "joblevel"),
        new("hrm.joblevel.delete", "Xóa/Vô hiệu hóa cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Delete, "joblevel"),

        // Vai trò & hệ thống
        new("system.role.read", "Xem danh sách vai trò", PermissionModule.System, PermissionAction.Read, "role"),
        new("system.role.create", "Thêm mới vai trò", PermissionModule.System, PermissionAction.Create, "role"),
        new("system.role.update", "Cập nhật quyền vai trò", PermissionModule.System, PermissionAction.Update, "role"),
        new("system.role.assign", "Gán vai trò cho nhân viên", PermissionModule.System, PermissionAction.Assign, "role"),
        new("system.user.resetpassword", "Đặt lại mật khẩu nhân viên", PermissionModule.System, PermissionAction.Update, "user"),

        // Quản lý quyền hệ thống
        new("system.permission.read", "Xem danh sách quyền", PermissionModule.System, PermissionAction.Read, "permission"),
        new("system.permission.create", "Thêm quyền mới", PermissionModule.System, PermissionAction.Create, "permission"),
        new("system.permission.update", "Cập nhật quyền", PermissionModule.System, PermissionAction.Update, "permission"),
        new("system.permission.delete", "Vô hiệu hóa quyền", PermissionModule.System, PermissionAction.Delete, "permission"),

        // Thông báo
        new("system.notification.event.read", "Xem loại thông báo", PermissionModule.System, PermissionAction.Read, "notification-event"),
        new("system.notification.event.create", "Tạo loại thông báo", PermissionModule.System, PermissionAction.Create, "notification-event"),
        new("system.notification.event.update", "Cập nhật loại thông báo", PermissionModule.System, PermissionAction.Update, "notification-event"),
        new("system.notification.event.delete", "Vô hiệu hóa loại thông báo", PermissionModule.System, PermissionAction.Delete, "notification-event"),
        new("system.notification.trigger.read", "Xem gán thông báo chức năng", PermissionModule.System, PermissionAction.Read, "notification-trigger"),
        new("system.notification.trigger.update", "Gán thông báo cho chức năng", PermissionModule.System, PermissionAction.Update, "notification-trigger"),

        // Chat
        new("chat.conversation.read", "Xem hội thoại/tin nhắn", PermissionModule.Chat, PermissionAction.Read, "conversation"),
        new("chat.conversation.create", "Tạo cuộc trò chuyện", PermissionModule.Chat, PermissionAction.Create, "conversation"),
        new("chat.conversation.update", "Cập nhật cuộc trò chuyện", PermissionModule.Chat, PermissionAction.Update, "conversation"),
        new("chat.conversation.delete", "Xóa cuộc trò chuyện", PermissionModule.Chat, PermissionAction.Delete, "conversation"),
        new("chat.message.create", "Gửi tin nhắn", PermissionModule.Chat, PermissionAction.Create, "message"),
        new("chat.message.update", "Chỉnh sửa tin nhắn", PermissionModule.Chat, PermissionAction.Update, "message"),
        new("chat.message.delete", "Xóa/Thu hồi tin nhắn", PermissionModule.Chat, PermissionAction.Delete, "message"),
        new("chat.member.manage", "Quản lý thành viên nhóm chat", PermissionModule.Chat, PermissionAction.Assign, "member"),
        new("chat.admin.manage", "Quản trị viên Chat", PermissionModule.Chat, PermissionAction.Approve, "chat-admin"),

        // Task
        new("task.item.read", "Xem danh sách công việc", PermissionModule.Task, PermissionAction.Read, "task"),
        new("task.item.create", "Tạo mới công việc", PermissionModule.Task, PermissionAction.Create, "task"),
        new("task.item.update", "Cập nhật công việc", PermissionModule.Task, PermissionAction.Update, "task"),
        new("task.item.delete", "Xóa công việc", PermissionModule.Task, PermissionAction.Delete, "task"),
        new("task.item.assign", "Giao việc cho nhân viên", PermissionModule.Task, PermissionAction.Assign, "task"),
        new("task.item.approve", "Phê duyệt công việc", PermissionModule.Task, PermissionAction.Approve, "task"),
        new("task.comment.create", "Bình luận trong công việc", PermissionModule.Task, PermissionAction.Create, "comment"),
        new("task.report.read", "Xem báo cáo hiệu suất công việc", PermissionModule.Task, PermissionAction.Read, "report"),
    ];

    public static readonly RoleSeed[] Roles =
    [
        new(Keys.RoleSuperAdmin, "Super Administrator", "Toàn quyền hệ thống, bỏ qua data scope", IsSystemRole: true, BypassDataScope: true),
        new(Keys.RoleHrAdmin, "HR Administrator", "Quản trị nhân sự và phòng ban", IsSystemRole: true, BypassDataScope: false),
        new(Keys.RoleEmployee, "Employee", "Vai trò nhân viên tiêu chuẩn", IsSystemRole: true, BypassDataScope: false),
    ];

    /// <summary>
    /// Map vai trò → danh sách mã permission. Super Admin nhận toàn bộ quyền (xử lý riêng trong DbInitializer).
    /// </summary>
    public static readonly Dictionary<string, string[]> RolePermissionCodes = new()
    {
        [Keys.RoleHrAdmin] =
        [
            "hrm.employee.read", "hrm.employee.create", "hrm.employee.update", "hrm.employee.delete",
            "hrm.department.read", "hrm.department.create", "hrm.department.update", "hrm.department.delete",
            "hrm.joblevel.read", "hrm.joblevel.create", "hrm.joblevel.update", "hrm.joblevel.delete",
            "system.role.assign", "system.user.resetpassword",
            "chat.admin.manage", "task.report.read",
            "chat.conversation.read", "chat.conversation.create", "chat.conversation.update", "chat.conversation.delete",
            "chat.message.create", "chat.message.update", "chat.message.delete", "chat.member.manage",
            "task.item.read", "task.item.create", "task.item.update", "task.item.delete", "task.item.assign", "task.item.approve", "task.comment.create"
        ],
        [Keys.RoleEmployee] =
        [
            "hrm.employee.read", "hrm.department.read", "hrm.joblevel.read",
            "chat.conversation.read", "chat.conversation.create", "chat.conversation.update",
            "chat.message.create", "chat.message.update", "chat.message.delete", "chat.member.manage",
            "task.item.read", "task.item.create", "task.item.update", "task.comment.create"
        ],
    };

    public static readonly UserSeed[] Users =
    [
        new(
            EmployeeCode: "ADMIN001",
            FullName: "System Administrator",
            Email: "admin@company.com",
            Password: Keys.DefaultPassword,
            DepartmentCode: Keys.DeptBoard,
            JobLevelName: Keys.JobDirector,
            DateOfJoin: new DateOnly(2026, 1, 1),
            Status: UserStatus.Active,
            RoleNames: [Keys.RoleSuperAdmin],
            ManageDepartmentCode: Keys.DeptBoard),
        new(
            EmployeeCode: "HR001",
            FullName: "Trần Thị Hương",
            Email: "hr.admin@company.com",
            Password: Keys.DefaultPassword,
            DepartmentCode: Keys.DeptHr,
            JobLevelName: Keys.JobManager,
            DateOfJoin: new DateOnly(2026, 1, 15),
            Status: UserStatus.Active,
            RoleNames: [Keys.RoleHrAdmin],
            ManageDepartmentCode: Keys.DeptHr),
        new(
            EmployeeCode: "EMP001",
            FullName: "Nguyễn Văn An",
            Email: "employee@company.com",
            Password: Keys.DefaultPassword,
            DepartmentCode: Keys.DeptIt,
            JobLevelName: Keys.JobStaff,
            DateOfJoin: new DateOnly(2026, 2, 1),
            Status: UserStatus.Active,
            RoleNames: [Keys.RoleEmployee]),
    ];
}
