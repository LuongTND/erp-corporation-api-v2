namespace Application;

public sealed record GetStoreHoursQuery(Guid StoreId) : IRequest<IEnumerable<StoreHoursResponse>>;
