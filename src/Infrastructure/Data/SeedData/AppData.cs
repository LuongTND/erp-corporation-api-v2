namespace Infrastructure;

public class AppData
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        await context.SaveChangesAsync();

        await UserData.SeedAdminAsync(context, hasher);
        // await StaffData.SeedAsync(context, hasher);
    }

    public static async Task SyncPermissionsAsync(ApplicationDbContext context, Assembly assembly)
    {
        var allDefs = typeof(PermissionInfoAttribute).Assembly
            .GetTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (Code: (string)f.GetRawConstantValue()!, Info: f.GetCustomAttribute<PermissionInfoAttribute>()))
            .Where(x => x.Info is not null)
            .Select(x => (x.Code, x.Info!.Name, x.Info.Description))
            .ToList();

        var allKeys = allDefs.Select(d => d.Code).ToHashSet();

        Console.WriteLine($"[SyncPermissions] Found {allDefs.Count} permission defs in assembly: {typeof(PermissionInfoAttribute).Assembly.FullName}");

        var existing = await context.Permissions.ToListAsync();

        foreach (var perm in existing)
        {
            var def = allDefs.FirstOrDefault(d => d.Code == perm.PermissionCode);
            if (def != default)
            {
                perm.PermissionName = def.Name;
                perm.Description    = def.Description;
            }
        }

        var existingKeys = existing.Select(p => p.PermissionCode).ToHashSet();
        var toAdd = allDefs
            .Where(d => !existingKeys.Contains(d.Code))
            .Select(d => new Permission
            {
                Id             = Guid.NewGuid(),
                PermissionCode = d.Code,
                PermissionName = d.Name,
                Description    = d.Description
            });

        context.Permissions.AddRange(toAdd);

        var obsolete = existing.Where(p => !allKeys.Contains(p.PermissionCode)).ToList();
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
