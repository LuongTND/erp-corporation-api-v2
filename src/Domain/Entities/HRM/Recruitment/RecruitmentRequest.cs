namespace Domain;

public class RecruitmentRequest : AuditableEntityBase<Guid>, ISoftDeletable
{
    public RecruitmentRequestContext RequestContext { get; set; }

    // HRM-046: Trưởng các BP  — set khi RequestContext = Department
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // HRM-045: QLCH — set khi RequestContext = Store
    public Guid? StoreId { get; set; }

    public string PositionTitle { get; set; } = string.Empty;

    public Guid RequestedByUserId { get; set; }
    public User? RequestedBy { get; set; }

    public int Headcount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? JobDescription { get; set; }

    public DateOnly? RequiredByDate { get; set; }

    public RecruitmentRequestStatus Status { get; set; } = RecruitmentRequestStatus.Draft;

    public string? RejectionNote { get; set; }
    public string? NeedMoreInfoNote { get; set; }

    public Guid? ApprovalRequestId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<Candidate> Candidates { get; set; } = [];
    public ICollection<JobPosting> JobPostings { get; set; } = [];
}
