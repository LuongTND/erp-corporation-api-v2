namespace Application;

public sealed record UpsertKpiEntryCommand(
    Guid UserId,
    Guid KpiMetricId,
    int Month,
    int Year,
    decimal ActualValue,
    decimal Score,
    string? Note) : IRequest<Guid>;
