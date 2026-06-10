using Application.Common.Models;
using Application.DTOs.Roles;

namespace Application.Interfaces.Services.Roles;

public interface IRoleService
{
    Task<RoleDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<RoleDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default);
    Task<PaginatedResult<PermissionDto>> GetPagedPermissionsAsync(PaginationQuery query, CancellationToken ct = default);
    Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<RoleDto> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken ct = default);
    Task UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
