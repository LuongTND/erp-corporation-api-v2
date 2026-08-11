namespace Application;

public sealed record UpdateKpiTemplateCommand(
    Guid Id,
    string Name,
    bool IsActive,
    List<KpiMetricDto> Metrics
) : IRequest<Unit>;
