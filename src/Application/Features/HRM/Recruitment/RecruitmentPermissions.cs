namespace Application;

public static class RecruitmentPermissions
{
    // ── Cấu hình người duyệt ────────────────────────────────────────────────
    [PermissionInfo("Xem cấu hình người duyệt tuyển dụng", "Xem danh sách người duyệt phiếu đề xuất tuyển dụng")]
    public const string ViewApproverConfig = "hrm:recruitment:approver-config:view";

    [PermissionInfo("Quản lý cấu hình người duyệt tuyển dụng", "Thêm, sửa, xóa cấu hình người duyệt phiếu đề xuất tuyển dụng")]
    public const string ManageApproverConfig = "hrm:recruitment:approver-config:manage";

    // ── Phiếu đề xuất ───────────────────────────────────────────────────────
    [PermissionInfo("Xem phiếu đề xuất tuyển dụng", "Xem danh sách và chi tiết phiếu đề xuất tuyển dụng")]
    public const string ViewRequest = "hrm:recruitment:request:view";

    [PermissionInfo("Tạo phiếu đề xuất tuyển dụng", "Tạo mới phiếu đề xuất tuyển dụng cho cửa hàng hoặc bộ phận")]
    public const string CreateRequest = "hrm:recruitment:request:create";

    [PermissionInfo("Cập nhật phiếu đề xuất tuyển dụng", "Chỉnh sửa thông tin phiếu đề xuất tuyển dụng")]
    public const string UpdateRequest = "hrm:recruitment:request:update";

    [PermissionInfo("Gửi phiếu đề xuất tuyển dụng", "Gửi phiếu đề xuất tuyển dụng đi để chờ duyệt")]
    public const string SubmitRequest = "hrm:recruitment:request:submit";

    [PermissionInfo("Duyệt phiếu tuyển dụng cấp 1", "Giám sát vùng / Trưởng bộ phận duyệt phiếu đề xuất tuyển dụng cấp 1")]
    public const string ApproveRequestLevel1 = "hrm:recruitment:request:approve-level1";

    [PermissionInfo("Duyệt phiếu tuyển dụng cấp 2", "Trưởng phòng Nhân sự phê duyệt cuối phiếu đề xuất tuyển dụng")]
    public const string ApproveRequest = "hrm:recruitment:request:approve";

    [PermissionInfo("Từ chối phiếu đề xuất tuyển dụng", "Từ chối phiếu đề xuất tuyển dụng kèm lý do")]
    public const string RejectRequest = "hrm:recruitment:request:reject";

    [PermissionInfo("Yêu cầu bổ sung thông tin tuyển dụng", "Yêu cầu người tạo bổ sung thêm thông tin phiếu đề xuất")]
    public const string RequestMoreInfo = "hrm:recruitment:request:more-info";

    [PermissionInfo("Xem lịch sử duyệt tuyển dụng", "Xem lịch sử phê duyệt các phiếu đề xuất tuyển dụng")]
    public const string ViewRequestHistory = "hrm:recruitment:request:history";

    // ── Ứng viên ────────────────────────────────────────────────────────────
    [PermissionInfo("Xem ứng viên", "Xem danh sách và thông tin chi tiết ứng viên")]
    public const string ViewCandidate = "hrm:recruitment:candidate:view";

    [PermissionInfo("Thêm ứng viên", "Nhập hồ sơ ứng viên mới vào hệ thống")]
    public const string CreateCandidate = "hrm:recruitment:candidate:create";

    [PermissionInfo("Cập nhật ứng viên", "Chỉnh sửa thông tin ứng viên")]
    public const string UpdateCandidate = "hrm:recruitment:candidate:update";

    [PermissionInfo("Tải lên CV ứng viên", "Đính kèm file CV của ứng viên")]
    public const string UploadCv = "hrm:recruitment:candidate:upload-cv";

    [PermissionInfo("Sơ loại ứng viên", "Thực hiện sơ loại hồ sơ ứng viên")]
    public const string ScreenCandidate = "hrm:recruitment:candidate:screen";

    [PermissionInfo("Chuyển ứng viên sang đánh giá", "Chuyển ứng viên đạt sơ loại sang bộ phận phỏng vấn")]
    public const string AssignCandidate = "hrm:recruitment:candidate:assign";

    [PermissionInfo("Đánh giá ứng viên", "Nhập kết quả đánh giá phỏng vấn ứng viên")]
    public const string EvaluateCandidate = "hrm:recruitment:candidate:evaluate";

    [PermissionInfo("Từ chối ứng viên", "Từ chối ứng viên kèm lý do")]
    public const string RejectCandidate = "hrm:recruitment:candidate:reject";

    [PermissionInfo("Chấp nhận ứng viên vào học việc", "Chuyển ứng viên đạt sang trạng thái học việc")]
    public const string HireCandidate = "hrm:recruitment:candidate:hire";

    // ── Phỏng vấn ───────────────────────────────────────────────────────────
    [PermissionInfo("Quản lý rule phỏng vấn", "Cấu hình quy tắc hẹn lịch phỏng vấn theo vùng / bộ phận")]
    public const string ManageInterviewRule = "hrm:recruitment:interview-rule:manage";

    [PermissionInfo("Quản lý lịch phỏng vấn", "Tạo và huỷ lịch phỏng vấn ứng viên")]
    public const string ManageInterviewSchedule = "hrm:recruitment:interview-schedule:manage";

    [PermissionInfo("Hoàn tất phỏng vấn", "Đánh dấu hoàn thành và nhập kết quả phỏng vấn")]
    public const string CompleteInterviewSchedule = "hrm:recruitment:interview-schedule:complete";

    // ── Tin tuyển dụng ──────────────────────────────────────────────────────
    [PermissionInfo("Quản lý tin tuyển dụng", "Tạo và quản lý tin đăng tuyển dụng")]
    public const string ManageJobPosting = "hrm:recruitment:posting:manage";

    [PermissionInfo("Tạo yêu cầu kênh tuyển phí", "Tạo yêu cầu đăng tuyển trên kênh trả phí")]
    public const string CreatePaidPosting = "hrm:recruitment:posting:paid-create";

    [PermissionInfo("Duyệt chi phí kênh tuyển", "Phê duyệt hoặc từ chối chi phí đăng tuyển kênh phí")]
    public const string ApprovePaidPosting = "hrm:recruitment:posting:paid-approve";
}
