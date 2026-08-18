namespace Application;

public sealed record CreateCounterCommand(Guid StoreId, string Name, string Code) : IRequest<Guid>;
