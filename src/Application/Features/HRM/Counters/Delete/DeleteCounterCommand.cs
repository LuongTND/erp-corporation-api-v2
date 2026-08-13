namespace Application;

public sealed record DeleteCounterCommand(Guid CounterId) : IRequest<Unit>;
