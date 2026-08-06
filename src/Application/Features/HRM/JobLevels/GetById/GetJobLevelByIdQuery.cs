namespace Application;

public sealed record GetJobLevelByIdQuery(Guid JobLevelId) : IRequest<JobLevelResponse>;
