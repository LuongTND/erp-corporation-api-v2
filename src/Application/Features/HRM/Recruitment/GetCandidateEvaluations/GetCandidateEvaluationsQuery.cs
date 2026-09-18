namespace Application;

public sealed record GetCandidateEvaluationsQuery(
    Guid ApplicationId,
    int Page = 1,
    int PageSize = 20
) : IRequest<QueryResult<CandidateEvaluationResponse>>;
