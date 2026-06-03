using Application.Interfaces.Repositories.Users;
using Application.Interfaces.Repositories.Roles;
using Application.Interfaces.Repositories.Departments;
using Application.Interfaces.Services.Auth;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Implementations.Services.Auth;

public class DataScopeService : IDataScopeService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public DataScopeService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IDepartmentRepository departmentRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<ScopeType> GetEffectiveScopeAsync(Guid userId, CancellationToken ct = default)
    {
        // 1. Nếu có Role bypass data scope -> All
        var hasBypass = await _roleRepository.HasBypassDataScopeRoleAsync(userId, ct);

        if (hasBypass)
            return ScopeType.All;

        // 2. Lấy scope mặc định từ JobLevel của User
        var user = await _userRepository.GetJobLevelScopeInfoAsync(userId, ct);
        var userScope = user?.JobLevel?.DefaultScopeType;

        return userScope ?? ScopeType.Own;
    }

    public async Task<IReadOnlyList<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct = default)
    {
        // Lấy phòng ban chính của user
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            return Array.Empty<Guid>();

        var primaryDeptId = user.DepartmentId;

        // Tải cây phòng ban đang hoạt động vào bộ nhớ để duyệt đệ quy (phù hợp với quy mô ERP trung bình)
        var allDepts = await _departmentRepository.GetAllAsync(ct);
        var activeDepts = allDepts
            .Where(d => d.IsActive)
            .Select(d => new { d.Id, d.ParentDepartmentId })
            .ToList();

        var result = new List<Guid> { primaryDeptId };
        var queue = new Queue<Guid>();
        queue.Enqueue(primaryDeptId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = activeDepts
                .Where(d => d.ParentDepartmentId == currentId)
                .Select(d => d.Id);

            foreach (var childId in children)
            {
                if (!result.Contains(childId))
                {
                    result.Add(childId);
                    queue.Enqueue(childId);
                }
            }
        }

        return result;
    }

    public async Task<IQueryable<User>> ApplyUserScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct = default)
    {
        var scope = await GetEffectiveScopeAsync(userId, ct);

        return scope switch
        {
            ScopeType.Own => query.Where(u => u.Id == userId),
            ScopeType.Team => query.Where(u => u.ManagerId == userId || u.Id == userId),
            ScopeType.Department => await ApplyDepartmentFilterAsync(query, userId, ct),
            ScopeType.All => query,
            _ => query.Where(u => u.Id == userId)
        };
    }

    private async Task<IQueryable<User>> ApplyDepartmentFilterAsync(IQueryable<User> query, Guid userId, CancellationToken ct)
    {
        var deptIds = await GetAccessibleDepartmentIdsAsync(userId, ct);
        return query.Where(u => deptIds.Contains(u.DepartmentId));
    }
}
