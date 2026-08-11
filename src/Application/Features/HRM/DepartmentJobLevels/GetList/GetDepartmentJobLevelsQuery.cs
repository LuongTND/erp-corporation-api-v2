namespace Application;

public sealed record GetDepartmentJobLevelsQuery(
    QueryInfo QueryInfo,
    Guid? DepartmentId = null
) : IRequest<QueryResult<DepartmentJobLevelResponse>>;
