namespace Application;

public sealed record GetAllJobPostingsQuery(
    QueryInfo QueryInfo,
    JobPostingCostStatus? CostStatus = null,
    Guid? RecruitmentRequestId = null
) : IRequest<QueryResult<JobPostingResponse>>;
