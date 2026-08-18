namespace Application;

public sealed record GetExpiringContractsQuery(int DaysAhead = 30) : IRequest<IEnumerable<EmploymentContractResponse>>;
