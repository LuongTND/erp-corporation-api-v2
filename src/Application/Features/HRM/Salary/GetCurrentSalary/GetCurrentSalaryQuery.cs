namespace Application;

public sealed record GetCurrentSalaryQuery(Guid UserId) : IRequest<SalaryRecordResponse?>;
