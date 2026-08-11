namespace Application;

public sealed record GetPayrollRunsQuery(int? Year) : IRequest<IReadOnlyList<PayrollRunResponse>>;
