namespace Domain;

/// <summary>
/// Cấu hình động cho quy trình phỏng vấn theo khu vực và loại đơn vị.
/// Priority cao hơn = rule cụ thể hơn, được ưu tiên áp dụng trước.
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

    // key permission dùng để xác định interviewer, e.g. "hrm:recruitment:candidate:evaluate"
    // role nào có permission này trong scope tương ứng sẽ được assign
    public string InterviewerRoleKey { get; set; } = string.Empty;

    public InterviewLocation Location { get; set; }

    // role key của người chốt lịch
    public string SchedulerRoleKey { get; set; } = string.Empty;

    // role key của người được thông báo khi ứng viên đạt
    public string NotifyRoleKey { get; set; } = string.Empty;

    // rule cụ thể hơn = priority cao hơn
    public int Priority { get; set; }

    public bool IsActive { get; set; } = true;
}
