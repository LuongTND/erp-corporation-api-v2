using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithDetailsScopedAsync(Guid id, Guid currentUserId, ScopeType scope, IReadOnlyList<Guid> accessibleDeptIds, CancellationToken ct = default);
    Task<List<User>> GetWithDetailsScopedAsync(Guid currentUserId, ScopeType scope, IReadOnlyList<Guid> accessibleDeptIds, CancellationToken ct = default);
    Task<bool> ExistsByEmployeeCodeAsync(string employeeCode, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailExcludeIdAsync(string email, Guid excludeId, CancellationToken ct = default);
    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetJobLevelScopeInfoAsync(Guid id, CancellationToken ct = default);
    Task<bool> HasActiveUsersWithJobLevelAsync(Guid jobLevelId, CancellationToken ct = default);
    Task<bool> HasActiveUsersInDepartmentAsync(Guid departmentId, CancellationToken ct = default);
    Task<User?> GetByIdWithAccountAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithAccountAndRolesAsync(Guid id, CancellationToken ct = default);
    Task AddUserRoleAsync(UserRole userRole, CancellationToken ct = default);
}
