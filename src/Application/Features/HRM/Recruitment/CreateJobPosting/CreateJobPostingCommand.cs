namespace Application;

public sealed record CreateJobPostingCommand(
    Guid RecruitmentRequestId,
    string Title,
    string Channel,
    string? PostUrl,
    decimal? EstimatedCost,
    DateTimeOffset? PostedAt,
    DateTimeOffset? ExpiresAt
) : IRequest<Guid>;
