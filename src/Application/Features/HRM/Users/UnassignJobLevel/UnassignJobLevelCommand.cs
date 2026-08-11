namespace Application;

public sealed record UnassignJobLevelCommand(Guid UserId) : IRequest<Unit>;
