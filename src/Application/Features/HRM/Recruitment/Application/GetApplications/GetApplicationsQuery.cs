namespace Application;

public sealed record GetApplicationsQuery(
    Guid JobPostingId,
    string? Stage,
    string? Search
) : IRequest<IEnumerable<ApplicationSummaryResponse>>;
