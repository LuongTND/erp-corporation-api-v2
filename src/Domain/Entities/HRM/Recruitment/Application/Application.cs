namespace Domain;

/// <summary>
/// Một lần ứng tuyển cụ thể của Applicant vào một JobPosting.
/// Đi qua 4 bước cố định: New → Scheduled → Interviewed → Hired | Rejected.
/// Một Applicant có thể có nhiều Application cho nhiều vị trí khác nhau.
/// </summary>
public class Application : AuditableEntityBase<Guid>, ISoftDeletable
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public Guid JobPostingId { get; set; }
    public JobPosting? JobPosting { get; set; }

    /// <summary>Nguồn CV cụ thể của ứng viên này (có thể khác Channel của JobPosting).</summary>
    public RecruitmentChannel SourceChannel { get; set; }

    public ApplicationStage Stage { get; set; } = ApplicationStage.New;

    public string? RejectionReason { get; set; }

    /// <summary>Ngày bắt đầu học việc sau khi chốt Hired.</summary>
    public DateOnly? TrialStartDate { get; set; }

    /// <summary>Set khi chuyển sang nhân viên chính thức (Onboarding).</summary>
    public Guid? ConvertedEmployeeId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<ApplicationStageHistory> StageHistory { get; set; } = [];
    public ICollection<InterviewSchedule> InterviewSchedules { get; set; } = [];
    public ICollection<ApplicationEvaluation> Evaluations { get; set; } = [];
}
