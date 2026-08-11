namespace Application;

public sealed class GetKpiEntriesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetKpiEntriesQuery, IReadOnlyList<KpiEntryResponse>>
{
    public async Task<IReadOnlyList<KpiEntryResponse>> Handle(GetKpiEntriesQuery query, CancellationToken ct)
    {
        var entries = await unitOfWork.Repository<KpiEntry>()
            .GetAllAsync(k =>
                k.Month == query.Month &&
                k.Year == query.Year &&
                (query.UserId == null || k.UserId == query.UserId.Value) &&
                (query.KpiMetricId == null || k.KpiMetricId == query.KpiMetricId.Value), ct);

        if (entries.Count == 0)
            return [];

        var userIds = entries.Select(e => e.UserId).Distinct().ToList();
        var metricIds = entries.Select(e => e.KpiMetricId).Distinct().ToList();

        var users = (await unitOfWork.Repository<User>()
            .GetAllAsync(u => userIds.Contains(u.Id), ct))
            .ToDictionary(u => u.Id);

        var metrics = (await unitOfWork.Repository<KpiMetric>()
            .GetAllAsync(m => metricIds.Contains(m.Id), ct))
            .ToDictionary(m => m.Id);

        return entries.Select(k => new KpiEntryResponse
        {
            Id = k.Id,
            UserId = k.UserId,
            FullName = users.TryGetValue(k.UserId, out var u) ? u.FullName : string.Empty,
            KpiMetricId = k.KpiMetricId,
            MetricName = metrics.TryGetValue(k.KpiMetricId, out var m) ? m.Name : string.Empty,
            Month = k.Month,
            Year = k.Year,
            ActualValue = k.ActualValue,
            Score = k.Score,
            Note = k.Note
        }).ToList();
    }
}
