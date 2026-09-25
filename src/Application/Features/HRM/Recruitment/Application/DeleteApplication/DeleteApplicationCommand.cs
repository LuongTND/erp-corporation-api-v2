namespace Application;

public sealed record DeleteApplicationCommand(Guid ApplicationId) : IRequest<Unit>;
