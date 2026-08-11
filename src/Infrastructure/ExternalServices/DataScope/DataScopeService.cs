namespace Infrastructure;

[RegisterService(typeof(IDataScopeService))]
public sealed class DataScopeService(IUnitOfWork unitOfWork) : IDataScopeService
{
    public async Task<ScopeType> GetUserScopeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await unitOfWork.Repository<User>()
            .FindAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userId));

        if (user.ScopeOverride.HasValue)
            return user.ScopeOverride.Value;

        if (!user.JobLevelId.HasValue)
            return ScopeType.Own;

        var jobLevel = await unitOfWork.Repository<JobLevel>()
            .FindAsync(j => j.Id == user.JobLevelId.Value, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", user.JobLevelId.Value));

        return jobLevel.DefaultScopeType;
    }

    public async Task<IReadOnlySet<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct = default)
    {
        var primaryDept = await unitOfWork.Repository<UserDepartment>()
            .FindAsync(ud => ud.UserId == userId && ud.IsPrimary && ud.IsActive, ct);

        if (primaryDept is null)
            return new HashSet<Guid>();

        // Load all departments once, then BFS in memory
        var allDepts = (await unitOfWork.Repository<Department>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            ct: ct)).Items.ToList();

        return BuildSubtree(primaryDept.DepartmentId, allDepts);
    }

    public async Task<IQueryable<User>> ApplyScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct = default)
    {
        var scope = await GetUserScopeAsync(userId, ct);

        return scope switch
        {
            ScopeType.Own => query.Where(u => u.Id == userId),
            ScopeType.Team => query.Where(u => u.Id == userId || u.ManagerId == userId),
            ScopeType.Department => await ApplyDepartmentScopeAsync(query, userId, ct),
            ScopeType.All => query,
            _ => query.Where(u => u.Id == userId)
        };
    }

    private async Task<IQueryable<User>> ApplyDepartmentScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct)
    {
        var deptIds = await GetAccessibleDepartmentIdsAsync(userId, ct);
        if (deptIds.Count == 0)
            return query.Where(u => u.Id == userId);

        // Join via UserDepartments — IsPrimary=true for primary dept filter
        var userIds = unitOfWork.Repository<UserDepartment>().Query()
            .Where(ud => deptIds.Contains(ud.DepartmentId) && ud.IsPrimary && ud.IsActive)
            .Select(ud => ud.UserId);

        return query.Where(u => userIds.Contains(u.Id));
    }

    private static IReadOnlySet<Guid> BuildSubtree(Guid rootId, List<Department> all)
    {
        var lookup = all.ToLookup(d => d.ParentDepartmentId);
        var result = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(rootId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);
            foreach (var child in lookup[current])
                queue.Enqueue(child.Id);
        }

        return result;
    }
}
