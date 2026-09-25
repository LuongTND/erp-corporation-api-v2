namespace Application;

public sealed record GetDepartmentsQuery(QueryInfo QueryInfo) : IRequest<QueryResult<DepartmentResponse>>;
