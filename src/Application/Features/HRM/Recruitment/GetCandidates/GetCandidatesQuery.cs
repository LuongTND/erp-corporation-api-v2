namespace Application;

public sealed record GetCandidatesQuery(
    QueryInfo QueryInfo,
    Guid? RecruitmentRequestId = null,
    CandidateStage? Stage = null
) : IRequest<QueryResult<CandidateResponse>>;
