namespace Application;

public sealed record HireCandidateCommand(Guid CandidateId, DateOnly? TrialStartDate) : IRequest<Unit>;
