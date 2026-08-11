namespace Application;

public sealed record GetKpiSummaryQuery(Guid UserId, int Month, int Year) : IRequest<KpiSummaryResponse>;
