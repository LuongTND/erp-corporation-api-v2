namespace Application;

public sealed class GetWorkHistoryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWorkHistoryQuery, IEnumerable<WorkHistoryResponse>>
{
    public async Task<IEnumerable<WorkHistoryResponse>> Handle(GetWorkHistoryQuery query, CancellationToken ct)
    {
        var items = await unitOfWork.Repository<WorkHistory>()
            .GetAllAsync(
                w => w.UserId == query.UserId && (query.ChangeType == null || w.ChangeType == query.ChangeType.Value),
                ct);

        return items.OrderByDescending(w => w.ChangedAt).Select(w => new WorkHistoryResponse
        {
            Id = w.Id,
            ChangeType = w.ChangeType.ToString(),
            ChangeTypeLabel = ChangeTypeLabel(w.ChangeType),
            OldValue = w.OldValue,
            NewValue = w.NewValue,
            Note = w.Note,
            ChangedBy = w.ChangedBy,
            ChangedAt = w.ChangedAt,
        });
    }

    private static string ChangeTypeLabel(WorkHistoryChangeType type) => type switch
    {
        WorkHistoryChangeType.Status       => "Trạng thái",
        WorkHistoryChangeType.JobLevel     => "Chức danh",
        WorkHistoryChangeType.Department   => "Bộ phận",
        WorkHistoryChangeType.Salary       => "Lương",
        WorkHistoryChangeType.ContractType => "Loại hợp đồng",
        WorkHistoryChangeType.Manager      => "Quản lý",
        _                                  => type.ToString(),
    };
}
