namespace Application;

public sealed class GetWorkHistoryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWorkHistoryQuery, IEnumerable<WorkHistoryResponse>>
{
    public async Task<IEnumerable<WorkHistoryResponse>> Handle(GetWorkHistoryQuery query, CancellationToken ct)
    {
        var includeDept = query.ChangeType == null || query.ChangeType == WorkHistoryChangeType.Department;
        var includeOther = query.ChangeType == null || query.ChangeType != WorkHistoryChangeType.Department;

        var workItems = includeOther
            ? await unitOfWork.Repository<WorkHistory>().GetAllAsync(
                w => w.UserId == query.UserId && (query.ChangeType == null || w.ChangeType == query.ChangeType.Value),
                ct)
            : [];

        var result = workItems.Select(w => new WorkHistoryResponse
        {
            Id = w.Id,
            ChangeType = w.ChangeType.ToString(),
            ChangeTypeLabel = ChangeTypeLabel(w.ChangeType),
            OldValue = w.OldValue,
            NewValue = w.NewValue,
            Note = w.Note,
            ChangedBy = w.ChangedBy,
            ChangedAt = w.ChangedAt,
        }).ToList();

        if (includeDept)
        {
            var userDepts = await unitOfWork.Repository<UserDepartment>()
                .GetAllAsync(ud => ud.UserId == query.UserId, ct);

            var deptIds = userDepts.Select(ud => ud.DepartmentId).Distinct().ToList();
            var deptNames = deptIds.Count > 0
                ? (await unitOfWork.Repository<Department>().GetAllAsync(d => deptIds.Contains(d.Id), ct))
                  .ToDictionary(d => d.Id, d => d.DepartmentName)
                : [];

            foreach (var ud in userDepts)
            {
                var name = deptNames.GetValueOrDefault(ud.DepartmentId, ud.DepartmentId.ToString());
                result.Add(new WorkHistoryResponse
                {
                    Id = ud.Id,
                    ChangeType = WorkHistoryChangeType.Department.ToString(),
                    ChangeTypeLabel = "Phòng ban",
                    OldValue = null,
                    NewValue = name,
                    ChangedAt = ud.StartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                });
                if (ud.EndDate.HasValue)
                    result.Add(new WorkHistoryResponse
                    {
                        Id = Guid.NewGuid(),
                        ChangeType = WorkHistoryChangeType.Department.ToString(),
                        ChangeTypeLabel = "Phòng ban",
                        OldValue = name,
                        NewValue = null,
                        ChangedAt = ud.EndDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                    });
            }
        }

        return result.OrderByDescending(w => w.ChangedAt);
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
