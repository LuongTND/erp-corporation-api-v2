namespace Application;

public sealed class GetContractsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetContractsQuery, IEnumerable<EmploymentContractResponse>>
{
    public async Task<IEnumerable<EmploymentContractResponse>> Handle(GetContractsQuery query, CancellationToken ct)
    {
        var contracts = await unitOfWork.Repository<EmploymentContract>()
            .GetAllAsync(c => c.UserId == query.UserId, ct);

        return contracts
            .OrderByDescending(c => c.StartDate)
            .Select(c => c.Adapt<EmploymentContractResponse>());
    }
}
