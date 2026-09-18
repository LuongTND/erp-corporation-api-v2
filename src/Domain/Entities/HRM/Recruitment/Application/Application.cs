namespace Domain;

public class Application : AuditableEntityBase<Guid>, ISoftDeletable
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public Guid? RecruitmentRequestId { get; set; }
    public RecruitmentRequest? RecruitmentRequest { get; set; }

    // null = HR tự add thủ công không qua tin đăng
    public Guid? JobPostingId { get; set; }
    public JobPosting? JobPosting { get; set; }

    public RecruitmentChannel SourceChannel { get; set; }

    public ApplicationStage Stage { get; set; } = ApplicationStage.New;

    public string? RejectionReason { get; set; }

    // set khi chốt lịch sau phỏng vấn đạt
    public DateOnly? TrialStartDate { get; set; }

    // set khi convert sang nhân viên chính thức
    public Guid? ConvertedEmployeeId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<ApplicationEvaluation> Evaluations { get; set; } = [];
    public ICollection<InterviewSchedule> InterviewSchedules { get; set; } = [];
    public ICollection<ApplicationStageHistory> StageHistory { get; set; } = [];
}
