namespace Application;

public sealed class GetPayrollRunByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPayrollRunByIdQuery, PayrollRunDetailResponse>
{
    public async Task<PayrollRunDetailResponse> Handle(GetPayrollRunByIdQuery query, CancellationToken ct)
    {
        var run = await unitOfWork.Repository<PayrollRun>()
            .FindAsync(r => r.Id == query.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("PayrollRun", query.Id));

        var entries = await unitOfWork.Repository<PayrollEntry>()
            .GetAllAsync(e => e.PayrollRunId == query.Id, ct);

        var userIds = entries.Select(e => e.UserId).Distinct().ToList();
        var users = userIds.Count > 0
            ? (await unitOfWork.Repository<User>().GetAllAsync(u => userIds.Contains(u.Id), ct))
                .ToDictionary(u => u.Id)
            : new Dictionary<Guid, User>();

        var entryResponses = entries.Select(e => new PayrollEntryResponse
        {
            Id = e.Id,
            UserId = e.UserId,
            FullName = users.TryGetValue(e.UserId, out var u) ? u.FullName : string.Empty,
            HourlyRateSnapshot = e.HourlyRateSnapshot,
            HoursWorked = e.HoursWorked,
            GrossPay = e.GrossPay,
            BonusAmount = e.BonusAmount,
            TotalDeductions = e.TotalDeductions,
            NetPay = e.NetPay,
            SocialInsurance = e.SocialInsurance,
            HealthInsurance = e.HealthInsurance,
            UnemploymentIns = e.UnemploymentIns,
            PersonalIncomeTax = e.PersonalIncomeTax,
            Note = e.Note
        }).ToList();

        return new PayrollRunDetailResponse
        {
            Id = run.Id,
            Month = run.Month,
            Year = run.Year,
            Status = run.Status.ToString(),
            Note = run.Note,
            CreatedAt = run.CreatedAt,
            Entries = entryResponses
        };
    }
}
