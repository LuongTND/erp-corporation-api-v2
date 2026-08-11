namespace Application;

public sealed record CreateKpiTemplateCommand(
    string Name,
    Guid DepartmentId,
    Guid? JobLevelId,
    List<KpiMetricDto> Metrics
) : IRequest<Guid>;
