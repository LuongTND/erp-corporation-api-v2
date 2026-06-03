using Application.DTOs.Roles;

namespace Application.Interfaces.Services.Roles;

public interface IRoleService
{
    Task<RoleDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync(CancellationToken ct = default);
    Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request, CancellationToken ct = default);
}
