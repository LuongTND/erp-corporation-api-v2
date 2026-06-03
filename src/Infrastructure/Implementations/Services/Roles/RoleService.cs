using Application.Common.Exceptions;
using Application.DTOs.Roles;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Roles;
using Application.Interfaces.Services.Roles;
using Domain.Entities;

namespace Infrastructure.Implementations.Services.Roles;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, ct);

        if (role == null)
            throw new NotFoundException("Không tìm thấy vai trò.");

        return MapToDto(role);
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await _roleRepository.GetAllWithPermissionsAsync(ct);

        return roles.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync(CancellationToken ct = default)
    {
        var permissions = await _roleRepository.GetAllPermissionsAsync(ct);

        return permissions.Select(MapPermissionToDto).ToList();
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var roleNameUpper = request.RoleName.ToUpperInvariant();

        if (await _roleRepository.ExistsByNameAsync(roleNameUpper, ct))
            throw new ConflictException($"Tên vai trò kĩ thuật '{request.RoleName}' đã tồn tại.");

        var role = Role.Create(
            roleNameUpper,
            request.DisplayName,
            request.Description,
            isSystemRole: false, // Thao tác từ UI chỉ tạo Custom Role
            bypassDataScope: request.BypassDataScope
        );

        await _roleRepository.AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(role.Id, ct);
    }

    public async Task UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, ct);

        if (role == null)
            throw new NotFoundException("Không tìm thấy vai trò.");

        // Kiểm tra danh sách PermissionIds đầu vào
        var validPermissions = await _roleRepository.GetPermissionsByIdsAsync(request.PermissionIds, ct);

        if (validPermissions.Count != request.PermissionIds.Count)
            throw new NotFoundException("Một hoặc nhiều quyền hạn không hợp lệ hoặc đã bị vô hiệu hóa.");

        // Xóa sạch các liên kết cũ và tạo liên kết mới
        await _roleRepository.UpdateRolePermissionsAsync(role, request.PermissionIds, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            DisplayName = role.DisplayName,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            BypassDataScope = role.BypassDataScope,
            IsActive = role.IsActive,
            Permissions = role.RolePermissions
                .Select(rp => MapPermissionToDto(rp.Permission))
                .ToList()
        };
    }

    private static PermissionDto MapPermissionToDto(Permission p)
    {
        return new PermissionDto
        {
            Id = p.Id,
            PermissionCode = p.PermissionCode,
            PermissionName = p.PermissionName,
            Module = p.Module,
            Action = p.Action,
            Resource = p.Resource,
            Description = p.Description,
            IsActive = p.IsActive
        };
    }
}
