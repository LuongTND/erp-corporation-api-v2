namespace Application;

public sealed record GetSalaryHistoryQuery(Guid UserId) : IRequest<IEnumerable<SalaryRecordResponse>>;
