namespace Application;

public sealed record GetStoresQuery(QueryInfo QueryInfo, Guid? RegionId = null) : IRequest<QueryResult<StoreResponse>>;
