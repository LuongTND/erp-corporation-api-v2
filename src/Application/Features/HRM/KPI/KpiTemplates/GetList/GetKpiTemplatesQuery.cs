namespace Application;

public sealed record GetKpiTemplatesQuery(
    QueryInfo QueryInfo,
    Guid? DepartmentId = null,
    Guid? JobLevelId = null
) : IRequest<QueryResult<KpiTemplateResponse>>;
