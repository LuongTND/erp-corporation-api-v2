namespace Application;

public sealed record GetJobPostingDetailQuery(Guid PostingId) : IRequest<JobPostingResponse>;
