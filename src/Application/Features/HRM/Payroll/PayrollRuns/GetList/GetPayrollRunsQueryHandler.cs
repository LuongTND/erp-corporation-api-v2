namespace Application;

public sealed class GetPayrollRunsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPayrollRunsQuery, IReadOnlyList<PayrollRunResponse>>
{
    public async Task<IReadOnlyList<PayrollRunResponse>> Handle(GetPayrollRunsQuery query, CancellationToken ct)
    {
        var runs = await unitOfWork.Repository<PayrollRun>()
            .GetAllAsync(r => query.Year == null || r.Year == query.Year.Value, ct);

        if (runs.Count == 0)
            return [];

        var runIds = runs.Select(r => r.Id).ToList();
        var entries = await unitOfWork.Repository<PayrollEntry>()
            .GetAllAsync(e => runIds.Contains(e.PayrollRunId), ct);

        var entryCountByRun = entries.GroupBy(e => e.PayrollRunId)
            .ToDictionary(g => g.Key, g => g.Count());
        var netPayByRun = entries.GroupBy(e => e.PayrollRunId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.NetPay));

        return runs
            .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
            .Select(r => new PayrollRunResponse
            {
                Id = r.Id,
                Month = r.Month,
                Year = r.Year,
                Status = r.Status.ToString(),
                Note = r.Note,
                EntryCount = entryCountByRun.GetValueOrDefault(r.Id, 0),
                TotalNetPay = netPayByRun.GetValueOrDefault(r.Id, 0),
                CreatedAt = r.CreatedAt
            })
            .ToList();
    }
}
