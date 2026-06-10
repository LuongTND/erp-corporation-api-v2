using Application.Common.Models;

namespace Application.Interfaces.Repositories.Permissions;

public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<PaginatedResult<Permission>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken ct = default);
    Task<bool> HasActiveRoleAssignmentsAsync(Guid permissionId, CancellationToken ct = default);
}
