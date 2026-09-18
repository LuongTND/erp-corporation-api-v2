namespace Application;

public sealed record GetCandidatesQuery(
    QueryInfo QueryInfo,
    Guid? RecruitmentRequestId = null,
    ApplicationStage? Stage = null
) : IRequest<QueryResult<CandidateResponse>>;
