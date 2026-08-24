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

        var userRoles = await unitOfWork.Repository<UserRole>()
            .GetAllAsync(ur => ur.UserId == userId && ur.IsActive && ur.RevokedAt == null
                && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow), ct);

        if (!userRoles.Any())
            return ScopeType.Own;

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = await unitOfWork.Repository<Domain.Role>()
            .GetAllAsync(r => roleIds.Contains(r.Id) && r.IsActive, ct);

        if (!roles.Any())
            return ScopeType.Own;

        return (ScopeType)roles.Max(r => (int)r.DefaultDataScope);
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
            ScopeType.Store => await ApplyStoreScopeAsync(query, userId, ct),
            ScopeType.Region => await ApplyRegionScopeAsync(query, userId, ct),
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

    private async Task<IQueryable<User>> ApplyStoreScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct)
    {
        var storeIds = unitOfWork.Repository<UserStore>().Query()
            .Where(us => us.UserId == userId && us.IsActive)
            .Select(us => us.StoreId);

        var userIdsInStores = unitOfWork.Repository<UserStore>().Query()
            .Where(us => storeIds.Contains(us.StoreId) && us.IsActive)
            .Select(us => us.UserId);

        return query.Where(u => userIdsInStores.Contains(u.Id));
    }

    private async Task<IQueryable<User>> ApplyRegionScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct)
    {
        var regionIds = unitOfWork.Repository<Store>().Query()
            .Where(s => s.ManagerId == userId && s.RegionId.HasValue)
            .Select(s => s.RegionId!.Value)
            .Distinct();

        // ponytail: also include region where user is Region.ManagerId
        var managedRegionIds = unitOfWork.Repository<Region>().Query()
            .Where(r => r.ManagerId == userId)
            .Select(r => r.Id);

        var allRegionIds = regionIds.Union(managedRegionIds);

        var storeIds = unitOfWork.Repository<Store>().Query()
            .Where(s => s.RegionId.HasValue && allRegionIds.Contains(s.RegionId!.Value))
            .Select(s => s.Id);

        var userIdsInRegion = unitOfWork.Repository<UserStore>().Query()
            .Where(us => storeIds.Contains(us.StoreId) && us.IsActive)
            .Select(us => us.UserId);

        return query.Where(u => userIdsInRegion.Contains(u.Id));
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
