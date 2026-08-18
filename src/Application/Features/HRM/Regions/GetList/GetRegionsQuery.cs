namespace Application;

public sealed record GetRegionsQuery(QueryInfo QueryInfo) : IRequest<QueryResult<RegionResponse>>;
