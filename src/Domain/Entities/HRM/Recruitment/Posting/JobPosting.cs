namespace Domain;

public class JobPosting : AuditableEntityBase<Guid>
{
    public Guid RecruitmentRequestId { get; set; }
    public RecruitmentRequest? RecruitmentRequest { get; set; }

    public string Title { get; set; } = string.Empty;

    public JobPostingStatus Status { get; set; } = JobPostingStatus.Draft;

    public RecruitmentChannel Channel { get; set; }

    public string? PostUrl { get; set; }

    // Nội dung public hiển thị trên nền tảng (khác JobDescription nội bộ trong RecruitmentRequest)
    public string? Description { get; set; }
    public string? Requirements { get; set; }

    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public bool SalaryVisible { get; set; } = false;

    public string? WorkingLocation { get; set; }

    public JobType JobType { get; set; } = JobType.FullTime;

    public decimal? EstimatedCost { get; set; }
    public JobPostingCostStatus CostStatus { get; set; } = JobPostingCostStatus.NotRequired;

    public Guid? CostApprovedByUserId { get; set; }
    public User? CostApprovedBy { get; set; }
    public DateTimeOffset? CostApprovedAt { get; set; }
    public string? CostRejectionNote { get; set; }

    public DateTimeOffset? PostedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public ICollection<Application> Applications { get; set; } = [];
}
