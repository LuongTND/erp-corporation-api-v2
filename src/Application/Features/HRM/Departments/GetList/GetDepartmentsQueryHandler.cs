namespace Application;

public sealed class GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepartmentsQuery, QueryResult<DepartmentResponse>>
{
    public async Task<QueryResult<DepartmentResponse>> Handle(GetDepartmentsQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var result = await unitOfWork.Repository<Department>().GetPagedAsync(
            query.QueryInfo,
            filter: d => search == null ||
                         d.DepartmentName.ToLower().Contains(search) ||
                         d.DepartmentCode.ToLower().Contains(search),
            orderBy: q => q.OrderBy(d => d.DepartmentCode),
            ct: ct);

        var depts = result.Items.ToList();
        if (depts.Count == 0)
            return new QueryResult<DepartmentResponse> { Items = [], TotalCount = result.TotalCount };

        var parentIds = depts
            .Where(d => d.ParentDepartmentId.HasValue)
            .Select(d => d.ParentDepartmentId!.Value)
            .Distinct().ToList();

        var managerIds = depts
            .Where(d => d.ManagerId.HasValue)
            .Select(d => d.ManagerId!.Value)
            .Distinct().ToList();

        var parents = parentIds.Count > 0
            ? (await unitOfWork.Repository<Department>().GetPagedAsync(
                new QueryInfo { Top = parentIds.Count, NeedTotalCount = false },
                filter: d => parentIds.Contains(d.Id),
                ct: ct)).Items.ToDictionary(d => d.Id)
            : new Dictionary<Guid, Department>();

        var managers = managerIds.Count > 0
            ? (await unitOfWork.Repository<User>().GetPagedAsync(
                new QueryInfo { Top = managerIds.Count, NeedTotalCount = false },
                filter: u => managerIds.Contains(u.Id),
                ct: ct)).Items.ToDictionary(u => u.Id)
            : new Dictionary<Guid, User>();

        var items = depts.Select(d => new DepartmentResponse
        {
            Id = d.Id,
            DepartmentName = d.DepartmentName,
            DepartmentCode = d.DepartmentCode,
            ParentDepartmentId = d.ParentDepartmentId,
            ParentDepartmentName = d.ParentDepartmentId.HasValue && parents.TryGetValue(d.ParentDepartmentId.Value, out var p) ? p.DepartmentName : null,
            ManagerId = d.ManagerId,
            ManagerName = d.ManagerId.HasValue && managers.TryGetValue(d.ManagerId.Value, out var m) ? m.FullName : null,
            Description = d.Description,
            IsActive = d.IsActive
        });

        return new QueryResult<DepartmentResponse> { Items = items, TotalCount = result.TotalCount };
    }
}
