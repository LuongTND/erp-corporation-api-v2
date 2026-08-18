namespace Application;

public sealed record UpdateCounterCommand(Guid CounterId, string Name, string Code) : IRequest<Unit>;
