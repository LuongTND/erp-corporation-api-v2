namespace Application;

public sealed record AssignStoreManagerCommand(Guid StoreId, Guid? ManagerId) : IRequest<Unit>;
