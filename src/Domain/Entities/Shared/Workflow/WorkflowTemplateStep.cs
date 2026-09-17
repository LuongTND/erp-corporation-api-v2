namespace Domain;

/// <summary>
/// Một bước trong mẫu quy trình duyệt.
/// Định nghĩa ai duyệt (ApproverType + ApproverId) và thứ tự thực hiện (StepOrder).
/// </summary>
public class WorkflowTemplateStep : EntityBase<Guid>
{
    public Guid TemplateId { get; set; }

    /// <summary>Thứ tự thực hiện, bắt đầu từ 1.</summary>
    public int StepOrder { get; set; }

    public string StepName { get; set; } = string.Empty;

    /// <summary>Loại người duyệt: SpecificUser, Role, hoặc OrgUnitManager.</summary>
    public WorkflowApproverType ApproverType { get; set; }

    /// <summary>
    /// UserId (SpecificUser), RoleId (Role), hoặc null (OrgUnitManager — resolve lúc runtime từ phiếu).
    /// </summary>
    public Guid? ApproverId { get; set; }

    public WorkflowTemplate Template { get; set; } = null!;
}
