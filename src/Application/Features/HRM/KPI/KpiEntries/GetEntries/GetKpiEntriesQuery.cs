namespace Application;

public sealed record GetKpiEntriesQuery(
    int Month,
    int Year,
    Guid? UserId,
    Guid? KpiMetricId) : IRequest<IReadOnlyList<KpiEntryResponse>>;
