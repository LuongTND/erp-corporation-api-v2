namespace Application;

public sealed record AssignCandidateToStoreCommand(Guid CandidateId) : IRequest<Unit>;
