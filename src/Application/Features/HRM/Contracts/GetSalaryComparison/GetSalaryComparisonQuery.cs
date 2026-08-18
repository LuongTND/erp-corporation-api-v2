namespace Application;

public sealed record GetSalaryComparisonQuery(Guid UserId) : IRequest<ContractSalaryComparisonResponse>;
