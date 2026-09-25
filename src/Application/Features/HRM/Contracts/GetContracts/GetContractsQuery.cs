namespace Application;

public sealed record GetContractsQuery(Guid UserId) : IRequest<IEnumerable<EmploymentContractResponse>>;
