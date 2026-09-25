namespace Application;

public sealed record GetJobPostingsQuery(Guid RecruitmentRequestId) : IRequest<IEnumerable<JobPostingResponse>>;
