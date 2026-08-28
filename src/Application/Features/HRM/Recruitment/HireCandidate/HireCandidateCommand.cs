namespace Application;

public sealed record HireCandidateCommand(Guid CandidateId) : IRequest<Unit>;
