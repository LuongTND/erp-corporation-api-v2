namespace Application;

public sealed record AssignCandidateToProductionCommand(Guid CandidateId) : IRequest<Unit>;
