using Domain.Entities;

namespace Application.Interfaces.Repositories.Roles;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken ct = default);
    Task<List<Role>> GetAllWithPermissionsAsync(CancellationToken ct = default);
    Task<List<Permission>> GetAllPermissionsAsync(CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<List<Permission>> GetPermissionsByIdsAsync(List<Guid> permissionIds, CancellationToken ct = default);
    Task<bool> HasBypassDataScopeRoleAsync(Guid userId, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default);
    Task UpdateRolePermissionsAsync(Role role, List<Guid> permissionIds, CancellationToken ct = default);
}
