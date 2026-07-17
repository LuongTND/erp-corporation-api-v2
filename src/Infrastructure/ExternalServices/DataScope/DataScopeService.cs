namespace Infrastructure;

[RegisterService(typeof(IDataScopeService))]
public class DataScopeService : IDataScopeService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDepartmentRepository _departmentRepository;

    private UserScopeContext? _cachedContext;
    private Guid _cachedUserId;

    public DataScopeService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IDepartmentRepository departmentRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<UserScopeContext> GetUserScopeContextAsync(Guid userId, CancellationToken ct = default)
    {
        if (_cachedContext != null && _cachedUserId == userId)
            return _cachedContext;

        var hasBypass = await _roleRepository.HasBypassDataScopeRoleAsync(userId, ct);
        if (hasBypass)
        {
            _cachedContext = new UserScopeContext(ScopeType.All, Array.Empty<Guid>());
            _cachedUserId = userId;
            return _cachedContext;
        }

        var user = await _userRepository.GetJobLevelScopeInfoAsync(userId, ct);
        var scope = user?.JobLevel?.DefaultScopeType ?? ScopeType.Own;

        IReadOnlyList<Guid> deptIds = scope == ScopeType.Department && user != null
            ? await BuildAccessibleDepartmentIdsAsync(user.DepartmentId, ct)
            : Array.Empty<Guid>();

        _cachedContext = new UserScopeContext(scope, deptIds);
        _cachedUserId = userId;
        return _cachedContext;
    }

    public async Task<ScopeType> GetEffectiveScopeAsync(Guid userId, CancellationToken ct = default)
    {
        var context = await GetUserScopeContextAsync(userId, ct);
        return context.Scope;
    }

    public async Task<IReadOnlyList<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct = default)
    {
        var context = await GetUserScopeContextAsync(userId, ct);
        return context.AccessibleDepartmentIds;
    }

    public async Task<IQueryable<User>> ApplyUserScopeAsync(IQueryable<User> query, Guid userId,
        CancellationToken ct = default)
    {
        var context = await GetUserScopeContextAsync(userId, ct);

        return context.Scope switch
        {
            ScopeType.Own => query.Where(u => u.Id == userId),
            ScopeType.Team => query.Where(u => u.ManagerId == userId || u.Id == userId),
            ScopeType.Department => query.Where(u => context.AccessibleDepartmentIds.Contains(u.DepartmentId)),
            ScopeType.All => query,
            _ => query.Where(u => u.Id == userId)
        };
    }

    private async Task<IReadOnlyList<Guid>> BuildAccessibleDepartmentIdsAsync(Guid primaryDeptId, CancellationToken ct)
    {
        var activeDepts = await _departmentRepository.GetActiveHierarchyAsync(ct);

        var result = new List<Guid> { primaryDeptId };
        var queue = new Queue<Guid>();
        queue.Enqueue(primaryDeptId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            foreach (var child in activeDepts.Where(d => d.ParentDepartmentId == currentId))
            {
                if (result.Contains(child.Id))
                    continue;

                result.Add(child.Id);
                queue.Enqueue(child.Id);
            }
        }

        return result;
    }
}