namespace Domain;

public class Candidate : AuditableEntityBase<Guid>, ISoftDeletable
{
    public Guid? RecruitmentRequestId { get; set; }
    public RecruitmentRequest? RecruitmentRequest { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CvUrl { get; set; }

    public RecruitmentChannel SourceChannel { get; set; }

    public CandidateStage Stage { get; set; } = CandidateStage.New;

    public string? RejectionReason { get; set; }

    public Guid? ConvertedEmployeeId { get; set; }

    // HRM-067: ngày bắt đầu học việc — set khi chốt lịch sau phỏng vấn đạt
    public DateOnly? TrialStartDate { get; set; }

    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<CandidateEvaluation> Evaluations { get; set; } = [];
    public ICollection<InterviewSchedule> InterviewSchedules { get; set; } = [];
}
