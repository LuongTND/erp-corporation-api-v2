namespace Application;

public sealed record AssignCandidateToProductionCommand(Guid ApplicationId) : IRequest<Unit>;
