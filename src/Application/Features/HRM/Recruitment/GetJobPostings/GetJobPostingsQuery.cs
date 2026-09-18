namespace Application;

public sealed record GetJobPostingsQuery(
    Guid RecruitmentRequestId,
    int Page = 1,
    int PageSize = 20
) : IRequest<QueryResult<JobPostingResponse>>;
