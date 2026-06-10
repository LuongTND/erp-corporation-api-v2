using Application.Common.Notifications;
using Infrastructure.Persistence.Seed;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbInitializer
{
    /// <summary>
    /// Chỉ seed dữ liệu mặc định khi database chưa có nhân sự (lần setup đầu).
    /// Không cập nhật lại dữ liệu mỗi lần API khởi động.
    /// </summary>
    public static async Task SeedIfEmptyAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        await SeedJobLevelsAsync(context);
        await SeedDepartmentsAsync(context);
        var permissions = await SeedPermissionsAsync(context);
        var roles = await SeedRolesAsync(context);
        await SeedRolePermissionsAsync(context, permissions, roles);
        await SeedUsersAsync(context, roles);
        await SeedNotificationsAsync(context);
    }

    /// <summary>
    /// Seed thông báo + quyền notification cho DB đã tồn tại (idempotent).
    /// </summary>
    public static async Task SeedNotificationsIfMissingAsync(AppDbContext context)
    {
        await SeedMissingNotificationPermissionsAsync(context);
        await SeedNotificationsAsync(context);
    }

    private static async Task SeedMissingNotificationPermissionsAsync(AppDbContext context)
    {
        var superAdmin = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == InitialData.Keys.RoleSuperAdmin);
        if (superAdmin == null)
            return;

        foreach (var seed in NotificationInitialData.NotificationPermissions)
        {
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.PermissionCode == seed.PermissionCode);
            if (permission == null)
            {
                permission = Permission.Create(
                    seed.PermissionCode,
                    seed.PermissionName,
                    seed.Module,
                    seed.Action,
                    seed.Resource);
                await context.Permissions.AddAsync(permission);
                await context.SaveChangesAsync();
            }

            var assigned = await context.RolePermissions.AnyAsync(rp =>
                rp.RoleId == superAdmin.Id && rp.PermissionId == permission.Id);
            if (!assigned)
            {
                await context.RolePermissions.AddAsync(RolePermission.Create(superAdmin.Id, permission.Id));
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedNotificationsAsync(AppDbContext context)
    {
        foreach (var seed in NotificationInitialData.EventTypes)
        {
            var entity = await context.NotificationEventTypes.FirstOrDefaultAsync(x => x.EventCode == seed.EventCode);
            if (entity == null)
            {
                entity = NotificationEventType.Create(
                    seed.Id,
                    seed.EventCode,
                    seed.Name,
                    seed.Module,
                    seed.DefaultTitleTemplate,
                    seed.DefaultBodyTemplate,
                    seed.Description);
                await context.NotificationEventTypes.AddAsync(entity);
            }
        }

        await context.SaveChangesAsync();

        foreach (var seed in NotificationInitialData.Triggers)
        {
            var entity = await context.NotificationTriggerBindings.FirstOrDefaultAsync(x => x.TriggerKey == seed.TriggerKey);
            if (entity == null)
            {
                entity = NotificationTriggerBinding.Create(
                    seed.Id,
                    seed.TriggerKey,
                    seed.Name,
                    seed.Module,
                    seed.EventTypeId,
                    seed.LinkUrlTemplate,
                    seed.Description,
                    seed.RecipientRulesJson ?? NotificationInitialData.DefaultAdminEventRecipientRulesJson);
                await context.NotificationTriggerBindings.AddAsync(entity);
            }
        }

        var triggersNeedingRules = await context.NotificationTriggerBindings
            .Where(x => x.RecipientRulesJson == "{}" || x.RecipientRulesJson == "")
            .ToListAsync();

        foreach (var trigger in triggersNeedingRules)
            trigger.SetRecipientRulesJson(NotificationInitialData.DefaultUserEventRecipientRulesJson);

        await context.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, JobLevel>> SeedJobLevelsAsync(AppDbContext context)
    {
        var map = new Dictionary<string, JobLevel>();

        foreach (var seed in InitialData.JobLevels)
        {
            var level = await context.JobLevels.FirstOrDefaultAsync(j => j.LevelName == seed.LevelName);
            if (level == null)
            {
                level = JobLevel.Create(
                    seed.LevelName,
                    seed.LevelOrder,
                    seed.DefaultScopeType,
                    seed.Description,
                    seed.BaseSalaryMin,
                    seed.BaseSalaryMax);
                await context.JobLevels.AddAsync(level);
            }

            map[seed.LevelName] = level;
        }

        await context.SaveChangesAsync();
        return map;
    }

    private static async Task<Dictionary<string, Department>> SeedDepartmentsAsync(AppDbContext context)
    {
        var map = new Dictionary<string, Department>();

        foreach (var seed in InitialData.Departments)
        {
            var dept = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentCode == seed.DepartmentCode);
            if (dept != null)
            {
                map[seed.DepartmentCode] = dept;
                continue;
            }

            Guid? parentId = null;
            if (seed.ParentDepartmentCode != null)
            {
                parentId = map[seed.ParentDepartmentCode].Id;
            }

            dept = Department.Create(
                seed.DepartmentName,
                seed.DepartmentCode,
                parentId,
                null,
                seed.Description);
            await context.Departments.AddAsync(dept);
            map[seed.DepartmentCode] = dept;
        }

        await context.SaveChangesAsync();
        return map;
    }

    private static async Task<Dictionary<string, Permission>> SeedPermissionsAsync(AppDbContext context)
    {
        var map = new Dictionary<string, Permission>();

        foreach (var seed in InitialData.Permissions)
        {
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.PermissionCode == seed.PermissionCode);
            if (permission == null)
            {
                permission = Permission.Create(
                    seed.PermissionCode,
                    seed.PermissionName,
                    seed.Module,
                    seed.Action,
                    seed.Resource);
                await context.Permissions.AddAsync(permission);
            }

            map[seed.PermissionCode] = permission;
        }

        await context.SaveChangesAsync();
        return map;
    }

    private static async Task<Dictionary<string, Role>> SeedRolesAsync(AppDbContext context)
    {
        var map = new Dictionary<string, Role>();

        foreach (var seed in InitialData.Roles)
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == seed.RoleName);
            if (role == null)
            {
                role = Role.Create(
                    seed.RoleName,
                    seed.DisplayName,
                    seed.Description,
                    seed.IsSystemRole,
                    seed.BypassDataScope);
                await context.Roles.AddAsync(role);
            }

            map[seed.RoleName] = role;
        }

        await context.SaveChangesAsync();
        return map;
    }

    private static async Task SeedRolePermissionsAsync(
        AppDbContext context,
        Dictionary<string, Permission> permissions,
        Dictionary<string, Role> roles)
    {
        var superAdmin = roles[InitialData.Keys.RoleSuperAdmin];
        foreach (var permission in permissions.Values)
        {
            var exists = await context.RolePermissions.AnyAsync(rp =>
                rp.RoleId == superAdmin.Id && rp.PermissionId == permission.Id);
            if (!exists)
            {
                await context.RolePermissions.AddAsync(RolePermission.Create(superAdmin.Id, permission.Id));
            }
        }

        foreach (var (roleName, codes) in InitialData.RolePermissionCodes)
        {
            var role = roles[roleName];
            foreach (var code in codes)
            {
                var permission = permissions[code];
                var exists = await context.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == role.Id && rp.PermissionId == permission.Id);
                if (!exists)
                {
                    await context.RolePermissions.AddAsync(RolePermission.Create(role.Id, permission.Id));
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext context, Dictionary<string, Role> roles)
    {
        var departments = await context.Departments.ToDictionaryAsync(d => d.DepartmentCode);
        var jobLevels = await context.JobLevels.ToDictionaryAsync(j => j.LevelName);

        foreach (var seed in InitialData.Users)
        {
            if (await context.Users.AnyAsync(u => u.EmployeeCode == seed.EmployeeCode))
                continue;

            var user = User.Create(
                seed.EmployeeCode,
                seed.FullName,
                seed.Email,
                departments[seed.DepartmentCode].Id,
                jobLevels[seed.JobLevelName].Id,
                seed.DateOfJoin,
                seed.Status);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var pwdHash = PasswordHasher.Hash(seed.Password);
            await context.UserAccounts.AddAsync(UserAccount.Create(user.Id, seed.Email, pwdHash));
            await context.UserDepartments.AddAsync(
                UserDepartment.Create(user.Id, departments[seed.DepartmentCode].Id, isPrimary: true, seed.DateOfJoin));

            foreach (var roleName in seed.RoleNames)
            {
                await context.UserRoles.AddAsync(UserRole.Create(user.Id, roles[roleName].Id));
            }

            if (seed.ManageDepartmentCode != null)
            {
                var dept = departments[seed.ManageDepartmentCode];
                dept.SetManager(user.Id);
                context.Departments.Update(dept);
            }

            await context.SaveChangesAsync();
        }
    }
}
