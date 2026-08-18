namespace Application;

public sealed record GetCountersQuery(Guid? StoreId, QueryInfo QueryInfo) : IRequest<QueryResult<CounterResponse>>;
