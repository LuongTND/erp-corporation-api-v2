namespace Domain;

public class JobPosting : AuditableEntityBase<Guid>
{
    public Guid RecruitmentRequestId { get; set; }
    public RecruitmentRequest? RecruitmentRequest { get; set; }

    public string Title { get; set; } = string.Empty;

    public RecruitmentChannel Channel { get; set; }

    public string? PostUrl { get; set; }

    public decimal? EstimatedCost { get; set; }
    public JobPostingCostStatus CostStatus { get; set; } = JobPostingCostStatus.NotRequired;

    public Guid? CostApprovedByUserId { get; set; }
    public User? CostApprovedBy { get; set; }
    public DateTimeOffset? CostApprovedAt { get; set; }
    public string? CostRejectionNote { get; set; }

    public DateTimeOffset? PostedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
