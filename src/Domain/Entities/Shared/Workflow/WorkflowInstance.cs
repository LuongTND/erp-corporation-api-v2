namespace Domain;

/// <summary>
/// Một lần chạy quy trình duyệt — được tạo khi một phiếu nghiệp vụ được submit.
/// Gắn với một phiếu cụ thể qua EntityType + EntityId.
/// </summary>
public class WorkflowInstance : AuditableEntityBase<Guid>
{
    public Guid TemplateId { get; set; }

    /// <summary>Loại nghiệp vụ, ví dụ: "RecruitmentRequest".</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Id của phiếu nghiệp vụ tương ứng.</summary>
    public Guid EntityId { get; set; }

    /// <summary>Scope thực tế của phiếu (Region/Department/All) — dùng để resolve OrgUnitManager ở các step sau.</summary>
    public WorkflowScopeType ScopeType { get; set; }

    /// <summary>Id của đơn vị tổ chức thực tế (regionId/deptId) — null nếu scope là All.</summary>
    public Guid? ScopeEntityId { get; set; }

    /// <summary>StepOrder của bước đang chờ duyệt hiện tại.</summary>
    public int CurrentStep { get; set; } = 1;

    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.InProgress;
    public DateTimeOffset? CompletedAt { get; set; }

    public WorkflowTemplate Template { get; set; } = null!;

    /// <summary>Lịch sử các task đã và đang xử lý trong lần chạy này.</summary>
    public ICollection<WorkflowTask> Tasks { get; set; } = [];
}
