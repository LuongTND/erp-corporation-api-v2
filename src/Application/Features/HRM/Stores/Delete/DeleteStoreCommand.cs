namespace Application;

public sealed record DeleteStoreCommand(Guid StoreId) : IRequest<Unit>;
