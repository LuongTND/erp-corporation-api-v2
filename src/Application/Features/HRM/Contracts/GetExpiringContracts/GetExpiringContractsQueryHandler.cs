namespace Application;

public sealed class GetExpiringContractsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExpiringContractsQuery, IEnumerable<EmploymentContractResponse>>
{
    public async Task<IEnumerable<EmploymentContractResponse>> Handle(GetExpiringContractsQuery query, CancellationToken ct)
    {
        var threshold = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(query.DaysAhead));
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var contracts = await unitOfWork.Repository<EmploymentContract>()
            .GetAllAsync(c =>
                c.Status == ContractStatus.Active &&
                c.EndDate.HasValue &&
                c.EndDate.Value >= today &&
                c.EndDate.Value <= threshold, ct);

        return contracts
            .OrderBy(c => c.EndDate)
            .Select(c => c.Adapt<EmploymentContractResponse>());
    }
}
