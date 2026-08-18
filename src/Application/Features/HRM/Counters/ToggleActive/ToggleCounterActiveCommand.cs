namespace Application;

public sealed record ToggleCounterActiveCommand(Guid CounterId) : IRequest<bool>;
