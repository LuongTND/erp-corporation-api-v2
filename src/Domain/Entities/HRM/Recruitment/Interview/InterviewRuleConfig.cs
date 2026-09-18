namespace Domain;

/// <summary>
/// Cấu hình động cho quy trình phỏng vấn theo khu vực và loại đơn vị.
/// Priority cao hơn = rule cụ thể hơn, được ưu tiên áp dụng trước.
/// Số vòng phỏng vấn và chi tiết từng vòng được định nghĩa trong Steps.
/// </summary>
public class InterviewRuleConfig : AuditableEntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;

    public RecruitmentRequestContext Context { get; set; }

    // null = áp dụng mọi khu vực (chỉ dùng khi Context = Store)
    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }

    // null = áp dụng mọi phòng ban (chỉ dùng khi Context = Department)
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // role key được thông báo khi ứng viên pass toàn bộ vòng phỏng vấn, null = không cần notify
    public string? NotifyRoleKey { get; set; }

    // rule cụ thể hơn = priority cao hơn
    public int Priority { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<InterviewRuleConfigStep> Steps { get; set; } = [];
}
