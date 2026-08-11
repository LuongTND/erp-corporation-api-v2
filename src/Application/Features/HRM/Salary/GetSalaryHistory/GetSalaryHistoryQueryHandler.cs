namespace Application;

public sealed class GetSalaryHistoryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSalaryHistoryQuery, IEnumerable<SalaryRecordResponse>>
{
    public async Task<IEnumerable<SalaryRecordResponse>> Handle(GetSalaryHistoryQuery query, CancellationToken ct)
    {
        var records = await unitOfWork.Repository<SalaryRecord>()
            .GetAllAsync(s => s.UserId == query.UserId, ct);

        return records
            .OrderByDescending(s => s.EffectiveFrom)
            .Select(s => new SalaryRecordResponse
            {
                Id = s.Id,
                UserId = s.UserId,
                FullName = string.Empty,
                HourlyRate = s.HourlyRate,
                EffectiveFrom = s.EffectiveFrom,
                EffectiveTo = s.EffectiveTo,
                Reason = s.Reason
            });
    }
}
