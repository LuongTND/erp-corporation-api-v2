namespace Application;

public static class WorkflowEntityTypes
{
    public const string RecruitmentRequest = "RecruitmentRequest";
    public const string LeaveRequest = "LeaveRequest";
    // public const string OvertimeRequest = "OvertimeRequest";
    public const string ExpenseRequest = "ExpenseRequest";

    public static readonly IReadOnlyList<WorkflowEntityTypeItem> All =
    [
        new(RecruitmentRequest, "Phiếu đề xuất tuyển dụng"),
        new(LeaveRequest,       "Phiếu xin nghỉ phép"),
        // new(OvertimeRequest,    "Phiếu làm thêm giờ"),
        new(ExpenseRequest,     "Phiếu hoàn ứng / chi phí"),
    ];
}

public sealed record WorkflowEntityTypeItem(string Value, string Label);

public static class WorkflowScopeTypes
{
    public static readonly IReadOnlyList<WorkflowScopeTypeItem> All =
    [
        new(WorkflowScopeType.Department, "Theo phòng ban"),
        new(WorkflowScopeType.Region,     "Theo vùng"),
        new(WorkflowScopeType.All,        "Toàn công ty"),
    ];
}

public sealed record WorkflowScopeTypeItem(WorkflowScopeType Value, string Label);

public static class WorkflowApproverTypes
{
    public static readonly IReadOnlyList<WorkflowApproverTypeItem> All =
    [
        new(WorkflowApproverType.SpecificUser,   "Người dùng cụ thể"),
        new(WorkflowApproverType.OrgUnitManager, "Trưởng đơn vị"),
        new(WorkflowApproverType.Role,           "Vai trò"),
    ];
}

public sealed record WorkflowApproverTypeItem(WorkflowApproverType Value, string Label);
