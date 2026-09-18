namespace Domain;

/// <summary>
/// Mẫu quy trình duyệt — định nghĩa các bước duyệt cho một loại nghiệp vụ (EntityType)
/// trong một phạm vi tổ chức (ScopeType + ScopeEntityId).
/// Hệ thống tra cứu theo 3 mức: khớp chính xác → wildcard cùng scope → toàn công ty (All).
/// </summary>
public class WorkflowTemplate : AuditableEntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Loại nghiệp vụ áp dụng, ví dụ: "RecruitmentRequest".</summary>
    public string EntityType { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>Phạm vi áp dụng: Department, Region, All, v.v.</summary>
    public WorkflowScopeType ScopeType { get; set; }

    /// <summary>
    /// Id của đơn vị cụ thể (phòng ban / vùng).
    /// Null = wildcard — áp dụng cho mọi đơn vị cùng ScopeType.
    /// </summary>
    public Guid? ScopeEntityId { get; set; }

    public ICollection<WorkflowTemplateStep> Steps { get; set; } = [];
}
