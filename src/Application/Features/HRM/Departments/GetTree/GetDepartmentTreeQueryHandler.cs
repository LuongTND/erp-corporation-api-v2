namespace Application;

public sealed class GetDepartmentTreeQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepartmentTreeQuery, IEnumerable<DepartmentTreeResponse>>
{
    public async Task<IEnumerable<DepartmentTreeResponse>> Handle(GetDepartmentTreeQuery query, CancellationToken ct)
    {
        var all = (await unitOfWork.Repository<Department>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            ct: ct)).Items.ToList();

        var managerIds = all
            .Where(d => d.ManagerId.HasValue)
            .Select(d => d.ManagerId!.Value)
            .Distinct().ToList();

        var managers = managerIds.Count > 0
            ? (await unitOfWork.Repository<User>().GetPagedAsync(
                new QueryInfo { Top = managerIds.Count, NeedTotalCount = false },
                filter: u => managerIds.Contains(u.Id),
                ct: ct)).Items.ToDictionary(u => u.Id)
            : new Dictionary<Guid, User>();

        var memberCountsRaw = await EntityFrameworkQueryableExtensions.ToListAsync(
            unitOfWork.Repository<UserDepartment>().Query()
                .Where(ud => ud.IsActive)
                .GroupBy(ud => ud.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() }),
            ct);
        var memberCounts = memberCountsRaw.ToDictionary(x => x.DepartmentId, x => x.Count);

        var lookup = all.ToLookup(d => d.ParentDepartmentId);
        return BuildTree(null, lookup, managers, memberCounts);
    }

    private static IEnumerable<DepartmentTreeResponse> BuildTree(
        Guid? parentId,
        ILookup<Guid?, Department> lookup,
        Dictionary<Guid, User> managers,
        Dictionary<Guid, int> memberCounts)
        => lookup[parentId].Select(d => new DepartmentTreeResponse
        {
            Id = d.Id,
            DepartmentName = d.DepartmentName,
            DepartmentCode = d.DepartmentCode,
            ParentDepartmentId = d.ParentDepartmentId,
            ManagerId = d.ManagerId,
            ManagerName = d.ManagerId.HasValue && managers.TryGetValue(d.ManagerId.Value, out var m) ? m.FullName : null,
            ManagerAvatarUrl = d.ManagerId.HasValue && managers.TryGetValue(d.ManagerId.Value, out var ma) ? ma.AvatarUrl : null,
            IsActive = d.IsActive,
            MemberCount = memberCounts.GetValueOrDefault(d.Id, 0),
            Children = [.. BuildTree(d.Id, lookup, managers, memberCounts)]
        });
}
