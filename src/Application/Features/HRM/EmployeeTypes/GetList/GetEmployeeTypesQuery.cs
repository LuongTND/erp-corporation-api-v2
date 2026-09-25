namespace Application;

public sealed record GetEmployeeTypesQuery(QueryInfo QueryInfo) : IRequest<QueryResult<EmployeeTypeResponse>>;
