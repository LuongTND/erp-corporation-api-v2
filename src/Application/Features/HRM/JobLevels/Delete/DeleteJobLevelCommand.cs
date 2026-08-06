namespace Application;

public sealed record DeleteJobLevelCommand(Guid JobLevelId) : IRequest<Unit>;
