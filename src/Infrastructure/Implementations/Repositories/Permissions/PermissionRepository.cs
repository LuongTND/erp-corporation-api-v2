namespace Infrastructure;

public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<Permission>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(p => p.PermissionCode)
            .ToPaginatedResultAsync(query, ct);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
    {
        var normalized = code.ToLowerInvariant();
        return await DbSet.AnyAsync(p => p.PermissionCode == normalized, ct);
    }

    public async Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken ct = default)
    {
        var normalized = code.ToLowerInvariant();
        return await DbSet.AnyAsync(p => p.PermissionCode == normalized && p.Id != excludeId, ct);
    }

    public async Task<bool> HasActiveRoleAssignmentsAsync(Guid permissionId, CancellationToken ct = default)
    {
        return await Context.RolePermissions
            .AnyAsync(rp => rp.PermissionId == permissionId && rp.Role.IsActive, ct);
    }
}