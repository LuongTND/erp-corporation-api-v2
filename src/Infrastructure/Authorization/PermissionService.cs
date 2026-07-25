namespace Infrastructure;

[RegisterService(typeof(IPermissionService))]
public sealed class PermissionService(ApplicationDbContext db, IRedisCacheService cache)
    : IPermissionService
{
    private static string CacheKey(Guid userId) => $"permissions:{userId}";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        var cached = await cache.GetRecordAsync<HashSet<string>>(CacheKey(userId));
        if (cached is not null) return cached;

        var now = DateTimeOffset.UtcNow;
        var roleIds = await db.Set<UserRole>()
            .AsNoTracking()
            .Where(ur => ur.UserId == userId
                      && ur.IsActive
                      && ur.RevokedAt == null
                      && (ur.ExpiresAt == null || ur.ExpiresAt > now))
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var permissions = roleIds.Count == 0
            ? []
            : await db.RolePermissions
                .AsNoTracking()
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.Permission!.PermissionCode)
                .ToHashSetAsync();

        await cache.SetRecordAsync(CacheKey(userId), permissions, CacheTtl);
        return permissions;
    }

    public async Task InvalidateCacheAsync(Guid roleId)
    {
        var userIds = await db.Set<UserRole>()
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId && ur.IsActive)
            .Select(ur => ur.UserId)
            .ToListAsync();

        if (userIds.Count == 0) return;
        await cache.RemoveManyAsync(userIds.Select(id => CacheKey(id)).ToArray());
    }
}
