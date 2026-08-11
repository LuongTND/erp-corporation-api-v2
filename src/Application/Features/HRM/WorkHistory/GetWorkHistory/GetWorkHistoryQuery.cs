namespace Application;

public sealed record GetWorkHistoryQuery(Guid UserId, WorkHistoryChangeType? ChangeType = null)
    : IRequest<IEnumerable<WorkHistoryResponse>>;
