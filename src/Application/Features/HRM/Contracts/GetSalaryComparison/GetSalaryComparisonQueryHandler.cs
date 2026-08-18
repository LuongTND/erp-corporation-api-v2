namespace Application;

public sealed class GetSalaryComparisonQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSalaryComparisonQuery, ContractSalaryComparisonResponse>
{
    public async Task<ContractSalaryComparisonResponse> Handle(GetSalaryComparisonQuery query, CancellationToken ct)
    {
        var activeContract = await unitOfWork.Repository<EmploymentContract>()
            .FindAsync(c => c.UserId == query.UserId && c.Status == ContractStatus.Active, ct);

        var currentSalary = await unitOfWork.Repository<SalaryRecord>()
            .FindAsync(s => s.UserId == query.UserId && s.EffectiveTo == null, ct);

        return new ContractSalaryComparisonResponse
        {
            UserId = query.UserId,
            ContractId = activeContract?.Id,
            ContractNumber = activeContract?.ContractNumber,
            ContractSalary = activeContract?.Salary,
            ActualHourlyRate = currentSalary?.HourlyRate,
            HasActiveContract = activeContract is not null,
            HasSalaryRecord = currentSalary is not null,
        };
    }
}
