namespace Domain;

public class RecruitmentRequest : AuditableEntityBase<Guid>, ISoftDeletable
{
    public RecruitmentRequestContext RequestContext { get; set; }

    // HRM-046: Trưởng các BP  — set khi RequestContext = Department
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // HRM-045: QLCH — set khi RequestContext = Store
    public Guid? StoreId { get; set; }
    public Store? Store { get; set; }

    public string PositionTitle { get; set; } = string.Empty;

    public Guid RequestedByUserId { get; set; }
    public User? RequestedBy { get; set; }

    public int Headcount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? JobDescription { get; set; }

    public DateOnly? RequiredByDate { get; set; }

    public string RequestCode { get; set; } = string.Empty;

    public RecruitmentRequestStatus Status { get; set; } = RecruitmentRequestStatus.Draft;

    public string? RejectionNote { get; set; }
    public string? NeedMoreInfoNote { get; set; }

    // Duyệt cấp 1: Giám sát vùng / Trưởng BP
    public Guid? Level1ApproverId { get; set; }
    public User? Level1Approver { get; set; }
    public DateTimeOffset? Level1ApprovedAt { get; set; }
    public string? Level1Note { get; set; }

    // Duyệt cấp 2: Trưởng phòng Nhân sự
    public Guid? Level2ApproverId { get; set; }
    public User? Level2Approver { get; set; }
    public DateTimeOffset? Level2ApprovedAt { get; set; }
    public string? Level2Note { get; set; }

    public Guid? WorkflowInstanceId { get; set; }
    public WorkflowInstance? WorkflowInstance { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<Candidate> Candidates { get; set; } = [];
    public ICollection<JobPosting> JobPostings { get; set; } = [];
}
