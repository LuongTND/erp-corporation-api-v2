namespace Domain;

/// <summary>
/// Bản ghi thực tế của một bước duyệt trong một lần chạy quy trình (WorkflowInstance).
/// Được tạo khi quy trình tiến đến bước tương ứng, không tạo trước tất cả cùng lúc.
/// </summary>
public class WorkflowTask : AuditableEntityBase<Guid>
{
    public Guid InstanceId { get; set; }

    /// <summary>Thứ tự bước, khớp với WorkflowTemplateStep.StepOrder.</summary>
    public int StepOrder { get; set; }

    public string StepName { get; set; } = string.Empty;

    /// <summary>UserId được assign — có giá trị khi ApproverType là SpecificUser hoặc OrgUnitManager.</summary>
    public Guid? AssignedTo { get; set; }

    /// <summary>RoleId được assign — có giá trị khi ApproverType là Role. Bất kỳ thành viên active của role đều có thể duyệt.</summary>
    public Guid? AssignedToRoleId { get; set; }

    public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Pending;
    public string? Note { get; set; }
    public DateTimeOffset? ActedAt { get; set; }

    /// <summary>UserId của người thực sự thao tác (approve/reject) — khác AssignedTo khi task assign theo role.</summary>
    public Guid? ActedByUserId { get; set; }

    /// <summary>Optimistic concurrency — ngăn double-approve khi nhiều người cùng role duyệt đồng thời.</summary>
    public byte[] RowVersion { get; set; } = null!;

    public WorkflowInstance Instance { get; set; } = null!;
}
