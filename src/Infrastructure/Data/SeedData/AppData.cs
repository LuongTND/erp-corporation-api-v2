namespace Infrastructure;

public class AppData
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        if (!await context.Roles.AnyAsync())
            await context.Roles.AddRangeAsync(RoleData.GetRoles());

        await context.SaveChangesAsync();

        await UserData.SeedAdminAsync(context, hasher);
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

        var toAdd = allKeys.Except(existing).Select(key => new Permission
        {
            Id = Guid.NewGuid(),
            PermissionCode = key,
            PermissionName = key,
            Module = PermissionModule.System,
            Action = PermissionAction.Read,
            Resource = key
        });

        context.Permissions.AddRange(toAdd);
        await context.SaveChangesAsync();

        await SyncAdminPermissionsAsync(context);
    }

    private static async Task SyncAdminPermissionsAsync(ApplicationDbContext context)
    {
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == RoleConstants.Admin);
        if (adminRole is null) return;

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
