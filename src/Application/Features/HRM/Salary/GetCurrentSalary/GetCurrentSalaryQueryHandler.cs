namespace Application;

public sealed class GetCurrentSalaryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCurrentSalaryQuery, SalaryRecordResponse?>
{
    public async Task<SalaryRecordResponse?> Handle(GetCurrentSalaryQuery query, CancellationToken ct)
    {
        var record = await unitOfWork.Repository<SalaryRecord>()
            .FindAsync(s => s.UserId == query.UserId && s.EffectiveTo == null, ct,
                s => s.User);

        if (record is null) return null;

        return MapToResponse(record);
    }

    internal static SalaryRecordResponse MapToResponse(SalaryRecord s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        FullName = s.User?.FullName ?? string.Empty,
        HourlyRate = s.HourlyRate,
        EffectiveFrom = s.EffectiveFrom,
        EffectiveTo = s.EffectiveTo,
        Reason = s.Reason
    };
}
