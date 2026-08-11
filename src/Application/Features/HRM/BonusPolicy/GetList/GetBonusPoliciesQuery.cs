namespace Application;

public sealed record GetBonusPoliciesQuery(QueryInfo QueryInfo) : IRequest<QueryResult<BonusPolicyResponse>>;
