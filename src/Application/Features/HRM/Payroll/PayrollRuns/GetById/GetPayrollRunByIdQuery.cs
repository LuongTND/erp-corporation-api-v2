namespace Application;

public sealed record GetPayrollRunByIdQuery(Guid Id) : IRequest<PayrollRunDetailResponse>;
