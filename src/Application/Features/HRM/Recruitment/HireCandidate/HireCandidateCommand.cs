namespace Application;

public sealed record HireCandidateCommand(Guid ApplicationId, DateOnly? TrialStartDate) : IRequest<Unit>;
