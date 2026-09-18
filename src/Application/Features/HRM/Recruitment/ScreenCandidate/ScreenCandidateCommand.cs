namespace Application;

public sealed record ScreenCandidateCommand(Guid ApplicationId) : IRequest<Unit>;
