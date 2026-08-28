namespace Application;

public sealed record GetCandidateEvaluationsQuery(
    Guid CandidateId,
    int Page = 1,
    int PageSize = 20
) : IRequest<QueryResult<CandidateEvaluationResponse>>;
