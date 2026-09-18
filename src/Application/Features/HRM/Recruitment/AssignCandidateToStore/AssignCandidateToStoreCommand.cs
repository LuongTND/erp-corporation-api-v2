namespace Application;

public sealed record AssignCandidateToStoreCommand(Guid ApplicationId) : IRequest<Unit>;
