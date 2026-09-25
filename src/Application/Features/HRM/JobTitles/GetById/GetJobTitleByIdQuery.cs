namespace Application;

public sealed record GetJobTitleByIdQuery(Guid JobTitleId) : IRequest<JobTitleResponse>;
