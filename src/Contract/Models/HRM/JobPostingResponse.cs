namespace Contract;

public class JobPostingSummaryResponse
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string Channel { get; init; } = string.Empty;
    public string? WorkLocation { get; init; }
    public string? PostUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? PostedAt { get; init; }
    public DateOnly? ExpiresAt { get; init; }
}

public sealed class JobPostingResponse : JobPostingSummaryResponse
{
    public Guid RecruitmentRequestId { get; init; }
    public string? Requirements { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public string? AssignedToName { get; init; }
    public int ApplicationCount { get; init; }
}
