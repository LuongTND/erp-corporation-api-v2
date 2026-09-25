namespace Application;

public sealed record ToggleStoreActiveCommand(Guid StoreId) : IRequest<bool>;
