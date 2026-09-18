namespace Contract;

public sealed class JobPostingResponse
{
    public Guid Id { get; init; }
    public Guid RecruitmentRequestId { get; init; }
    public string? RequestCode { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Channel { get; init; } = string.Empty;
    public string? PostUrl { get; init; }
    public decimal? EstimatedCost { get; init; }
    public string CostStatus { get; init; } = string.Empty;
    public Guid? CostApprovedByUserId { get; init; }
    public string? CostApprovedByName { get; init; }
    public DateTimeOffset? CostApprovedAt { get; init; }
    public string? CostRejectionNote { get; init; }
    public DateTimeOffset? PostedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
