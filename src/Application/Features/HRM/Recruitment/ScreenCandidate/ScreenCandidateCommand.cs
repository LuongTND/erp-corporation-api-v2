namespace Application;

public sealed record ScreenCandidateCommand(Guid CandidateId) : IRequest<Unit>;
