namespace Application;

public sealed class GetKpiSummaryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetKpiSummaryQuery, KpiSummaryResponse>
{
    public async Task<KpiSummaryResponse> Handle(GetKpiSummaryQuery query, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindAsync(u => u.Id == query.UserId && u.IsActive, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", query.UserId));

        var entries = await unitOfWork.Repository<KpiEntry>()
            .GetAllAsync(k => k.UserId == query.UserId && k.Month == query.Month && k.Year == query.Year, ct);

        var metricIds = entries.Select(e => e.KpiMetricId).Distinct().ToList();
        var metrics = metricIds.Count > 0
            ? (await unitOfWork.Repository<KpiMetric>().GetAllAsync(m => metricIds.Contains(m.Id), ct))
                .ToDictionary(m => m.Id)
            : new Dictionary<Guid, KpiMetric>();

        var entryResponses = entries.Select(k => new KpiEntryResponse
        {
            Id = k.Id,
            UserId = k.UserId,
            FullName = user.FullName,
            KpiMetricId = k.KpiMetricId,
            MetricName = metrics.TryGetValue(k.KpiMetricId, out var m) ? m.Name : string.Empty,
            Month = k.Month,
            Year = k.Year,
            ActualValue = k.ActualValue,
            Score = k.Score,
            Note = k.Note
        }).ToList();

        return new KpiSummaryResponse
        {
            UserId = query.UserId,
            FullName = user.FullName,
            Month = query.Month,
            Year = query.Year,
            TotalScore = entryResponses.Count > 0 ? entryResponses.Average(e => e.Score) : 0,
            Entries = entryResponses
        };
    }
}
