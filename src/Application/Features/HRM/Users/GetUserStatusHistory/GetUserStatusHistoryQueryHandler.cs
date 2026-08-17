namespace Application;

public sealed class GetUserStatusHistoryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserStatusHistoryQuery, IEnumerable<UserStatusHistoryResponse>>
{
    public async Task<IEnumerable<UserStatusHistoryResponse>> Handle(
        GetUserStatusHistoryQuery query, CancellationToken ct)
    {
        var items = await unitOfWork.Repository<WorkHistory>()
            .GetAllAsync(w => w.UserId == query.UserId && w.ChangeType == WorkHistoryChangeType.Status, ct);

        return items.OrderByDescending(w => w.ChangedAt).Select(w => new UserStatusHistoryResponse(
            ChangedAt: w.ChangedAt,
            OldStatus: w.OldValue ?? string.Empty,
            NewStatus: w.NewValue ?? string.Empty,
            Note: w.Note,
            ChangedBy: w.ChangedBy
        ));
    }
}
