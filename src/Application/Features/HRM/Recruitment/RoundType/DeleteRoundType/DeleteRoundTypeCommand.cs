namespace Application;

public sealed record DeleteRoundTypeCommand(Guid Id) : IRequest<Unit>;
