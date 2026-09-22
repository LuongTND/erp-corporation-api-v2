namespace Domain;

/// <summary>
/// Phiếu đề xuất tuyển dụng — gốc của toàn bộ vòng đời tuyển dụng.
/// Được tạo bởi cửa hàng hoặc phòng ban khi phát sinh nhu cầu nhân sự,
/// trải qua workflow duyệt (Trưởng BP → TPNS) trước khi HR tiếp nhận.
/// Sau khi Approved, HR tạo JobPosting và bắt đầu nhận CV ứng viên.
/// </summary>
public class RecruitmentRequest : AuditableEntityBase<Guid>, ISoftDeletable
{
    public RecruitmentRequestContext RequestContext { get; set; }

    /// <summary>Set khi RequestContext = Department (Trưởng các BP).</summary>
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    /// <summary>Set khi RequestContext = Store (QLCH).</summary>
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
    public string? CancelNote { get; set; }

    public Guid? WorkflowInstanceId { get; set; }
    public WorkflowInstance? WorkflowInstance { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<Application> Applications { get; set; } = [];
    public ICollection<JobPosting> JobPostings { get; set; } = [];
}
