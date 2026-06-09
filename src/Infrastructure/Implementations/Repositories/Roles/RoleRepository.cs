using Application.Interfaces.Repositories.Roles;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Roles;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<List<Role>> GetAllWithPermissionsAsync(CancellationToken ct = default)
    {
        return await DbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync(ct);
    }

    public async Task<List<Permission>> GetAllPermissionsAsync(CancellationToken ct = default)
    {
        return await Context.Permissions
            .Where(p => p.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(r => r.RoleName == name, ct);
    }

    public async Task<List<Permission>> GetPermissionsByIdsAsync(List<Guid> permissionIds, CancellationToken ct = default)
    {
        return await Context.Permissions
            .Where(p => permissionIds.Contains(p.Id) && p.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> HasBypassDataScopeRoleAsync(Guid userId, CancellationToken ct = default)
    {
        return await Context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive && ur.RevokedAt == null)
            .AnyAsync(ur => ur.Role.IsActive && ur.Role.BypassDataScope, ct);
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default)
    {
        var isSuperAdmin = await HasBypassDataScopeRoleAsync(userId, ct);
        if (isSuperAdmin)
            return true;

        var normalizedPermission = permissionCode.ToLowerInvariant();

        return await Context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive && ur.RevokedAt == null && ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Where(p => p.IsActive)
            .AnyAsync(p => p.PermissionCode == normalizedPermission, ct);
    }

    public async Task UpdateRolePermissionsAsync(Role role, List<Guid> permissionIds, CancellationToken ct = default)
    {
        Context.RolePermissions.RemoveRange(role.RolePermissions);

        foreach (var permissionId in permissionIds)
        {
            var rp = RolePermission.Create(role.Id, permissionId);
            await Context.RolePermissions.AddAsync(rp, ct);
        }
    }

    public async Task<bool> HasActiveUsersInRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        return await Context.UserRoles
            .AnyAsync(ur => ur.RoleId == roleId && ur.IsActive && ur.User.IsActive, ct);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(r => r.RoleName == name, ct);
    }

    public async Task<List<Role>> GetActiveRolesByIdsAsync(List<Guid> roleIds, CancellationToken ct = default)
    {
        return await DbSet
            .Where(r => roleIds.Contains(r.Id) && r.IsActive)
            .ToListAsync(ct);
    }
}
