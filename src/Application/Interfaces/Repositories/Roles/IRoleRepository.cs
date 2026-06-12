using Application.Common.Models;

namespace Application.Interfaces.Repositories.Roles;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<Role>> GetPagedWithPermissionsAsync(PaginationQuery query, CancellationToken ct = default);
    Task<PaginatedResult<Permission>> GetPagedPermissionsAsync(PaginationQuery query, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<List<Permission>> GetPermissionsByIdsAsync(List<Guid> permissionIds, CancellationToken ct = default);
    Task<bool> HasBypassDataScopeRoleAsync(Guid userId, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default);
    Task<List<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken ct = default);
    Task UpdateRolePermissionsAsync(Role role, List<Guid> permissionIds, CancellationToken ct = default);
    Task<bool> HasActiveUsersInRoleAsync(Guid roleId, CancellationToken ct = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<List<Role>> GetActiveRolesByIdsAsync(List<Guid> roleIds, CancellationToken ct = default);
}
