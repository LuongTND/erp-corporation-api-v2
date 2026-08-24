namespace Infrastructure;

public class AppData
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        await context.SaveChangesAsync();

        await UserData.SeedAdminAsync(context, hasher);
        await StaffData.SeedAsync(context, hasher);
    }

    public static async Task SyncPermissionsAsync(ApplicationDbContext context, Assembly assembly)
    {
        var allKeys = assembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(m => m.GetCustomAttributes<HasPermissionAttribute>())
            .Select(a => a.Permission)
            .ToHashSet();

        var existing = await context.Permissions.Select(p => p.PermissionCode).ToHashSetAsync();

        // Update PermissionName + Description for existing permissions
        var existingPerms = await context.Permissions.ToListAsync();
        foreach (var perm in existingPerms)
        {
            if (PermissionNames.Map.TryGetValue(perm.PermissionCode, out var info))
            {
                perm.PermissionName = info.Name;
                perm.Description = info.Description;
            }
        }

        var toAdd = allKeys.Except(existing).Select(key =>
        {
            var (name, desc) = PermissionNames.Map.TryGetValue(key, out var info)
                ? info
                : (key, null);
            return new Permission
            {
                Id = Guid.NewGuid(),
                PermissionCode = key,
                PermissionName = name,
                Description = desc
            };
        });

        context.Permissions.AddRange(toAdd);

        var obsolete = await context.Permissions
            .Where(p => !allKeys.Contains(p.PermissionCode))
            .ToListAsync();

        context.Permissions.RemoveRange(obsolete);

        await context.SaveChangesAsync();

        await SyncAdminPermissionsAsync(context);
    }

    private static async Task SyncAdminPermissionsAsync(ApplicationDbContext context)
    {
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == RoleConstants.Admin);
        if (adminRole is null) return;

        if (adminRole.DefaultDataScope != ScopeType.All)
        {
            adminRole.DefaultDataScope = ScopeType.All;
            await context.SaveChangesAsync();
        }

        var allPermissionIds = await context.Permissions.Select(p => p.Id).ToListAsync();
        var existingPermissionIds = await context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync();

        var toAssign = allPermissionIds
            .Where(pid => !existingPermissionIds.Contains(pid))
            .Select(pid => new RolePermission { RoleId = adminRole.Id, PermissionId = pid });

        context.RolePermissions.AddRange(toAssign);
        await context.SaveChangesAsync();
    }
}
