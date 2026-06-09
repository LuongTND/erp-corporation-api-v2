using Domain.Entities;
using Domain.Enums;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Seed JobLevels
        JobLevel? directorLevel = null;
        JobLevel? managerLevel = null;
        JobLevel? staffLevel = null;

        if (!await context.JobLevels.AnyAsync())
        {
            directorLevel = JobLevel.Create("Director", 1, ScopeType.All, "Director level with access to all data.");
            managerLevel = JobLevel.Create("Manager", 2, ScopeType.Department, "Manager level with department scope.");
            staffLevel = JobLevel.Create("Staff", 3, ScopeType.Own, "Staff level with own scope.");

            await context.JobLevels.AddRangeAsync(directorLevel, managerLevel, staffLevel);
            await context.SaveChangesAsync();
        }
        else
        {
            directorLevel = await context.JobLevels.FirstAsync(j => j.LevelName == "Director");
            managerLevel = await context.JobLevels.FirstAsync(j => j.LevelName == "Manager");
            staffLevel = await context.JobLevels.FirstAsync(j => j.LevelName == "Staff");
        }

        // 2. Seed Departments
        Department? rootDept = null;
        Department? hrDept = null;

        if (!await context.Departments.AnyAsync())
        {
            rootDept = Department.Create("Ban Giám Đốc", "BGĐ", null, null, "Hội đồng quản trị và Ban giám đốc");
            await context.Departments.AddAsync(rootDept);
            await context.SaveChangesAsync();

            hrDept = Department.Create("Phòng Nhân Sự", "HR", rootDept.Id, null, "Quản lý nhân sự và tiền lương");
            await context.Departments.AddAsync(hrDept);
            await context.SaveChangesAsync();
        }
        else
        {
            rootDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "BGĐ");
            hrDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "HR");
        }

        // 3. Seed Permissions
        var permissionsToSeed = new List<(string Code, string Name, PermissionModule Module, PermissionAction Action, string Resource)>
        {
            // Employees
            ("hrm.employee.read", "Xem danh sách nhân viên", PermissionModule.Hrm, PermissionAction.Read, "employee"),
            ("hrm.employee.create", "Thêm mới nhân viên", PermissionModule.Hrm, PermissionAction.Create, "employee"),
            ("hrm.employee.update", "Sửa thông tin nhân viên", PermissionModule.Hrm, PermissionAction.Update, "employee"),
            ("hrm.employee.delete", "Xóa/Vô hiệu hóa nhân viên", PermissionModule.Hrm, PermissionAction.Delete, "employee"),
            
            // Departments
            ("hrm.department.read", "Xem danh sách phòng ban", PermissionModule.Hrm, PermissionAction.Read, "department"),
            ("hrm.department.create", "Thêm mới phòng ban", PermissionModule.Hrm, PermissionAction.Create, "department"),
            ("hrm.department.update", "Sửa thông tin phòng ban", PermissionModule.Hrm, PermissionAction.Update, "department"),
            ("hrm.department.delete", "Xóa phòng ban", PermissionModule.Hrm, PermissionAction.Delete, "department"),
            
            // Job Levels
            ("hrm.joblevel.read", "Xem danh sách cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Read, "joblevel"),
            ("hrm.joblevel.create", "Thêm mới cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Create, "joblevel"),
            ("hrm.joblevel.update", "Sửa thông tin cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Update, "joblevel"),
            ("hrm.joblevel.delete", "Xóa/Vô hiệu hóa cấp bậc chức danh", PermissionModule.Hrm, PermissionAction.Delete, "joblevel"),
            
            // Roles & System
            ("system.role.read", "Xem danh sách vai trò", PermissionModule.System, PermissionAction.Read, "role"),
            ("system.role.create", "Thêm mới vai trò", PermissionModule.System, PermissionAction.Create, "role"),
            ("system.role.update", "Cập nhật quyền vai trò", PermissionModule.System, PermissionAction.Update, "role"),
            ("system.role.assign", "Gán vai trò cho nhân viên", PermissionModule.System, PermissionAction.Assign, "role"),
            ("system.user.resetpassword", "Đặt lại mật khẩu nhân viên", PermissionModule.System, PermissionAction.Update, "user"),
        };

        var allDbPermissions = new List<Permission>();
        foreach (var pInfo in permissionsToSeed)
        {
            var p = await context.Permissions.FirstOrDefaultAsync(per => per.PermissionCode == pInfo.Code);
            if (p == null)
            {
                p = Permission.Create(pInfo.Code, pInfo.Name, pInfo.Module, pInfo.Action, pInfo.Resource);
                await context.Permissions.AddAsync(p);
            }
            allDbPermissions.Add(p);
        }
        await context.SaveChangesAsync();

        // 4. Seed Roles
        Role? superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "ROLE_SUPER_ADMIN");
        if (superAdminRole == null)
        {
            superAdminRole = Role.Create("ROLE_SUPER_ADMIN", "Super Administrator", "Full system control with bypass data scope.", isSystemRole: true, bypassDataScope: true);
            await context.Roles.AddAsync(superAdminRole);
        }

        Role? hrAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "ROLE_HR_ADMIN");
        if (hrAdminRole == null)
        {
            hrAdminRole = Role.Create("ROLE_HR_ADMIN", "HR Administrator", "Human resource management role.", isSystemRole: true, bypassDataScope: false);
            await context.Roles.AddAsync(hrAdminRole);
        }

        Role? employeeRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "ROLE_EMPLOYEE");
        if (employeeRole == null)
        {
            employeeRole = Role.Create("ROLE_EMPLOYEE", "Employee", "Standard employee role.", isSystemRole: true, bypassDataScope: false);
            await context.Roles.AddAsync(employeeRole);
        }
        await context.SaveChangesAsync();

        // 5. Seed RolePermissions (Map all permissions to Super Admin & HR Admin)
        foreach (var p in allDbPermissions)
        {
            var exists = await context.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRole.Id && rp.PermissionId == p.Id);
            if (!exists)
            {
                await context.RolePermissions.AddAsync(RolePermission.Create(superAdminRole.Id, p.Id));
            }
        }

        // HR Admin gets employee, department, joblevel, role assignment, and reset password permissions
        foreach (var p in allDbPermissions.Where(per => per.PermissionCode.StartsWith("hrm.") || per.PermissionCode == "system.role.assign" || per.PermissionCode == "system.user.resetpassword"))
        {
            var exists = await context.RolePermissions.AnyAsync(rp => rp.RoleId == hrAdminRole.Id && rp.PermissionId == p.Id);
            if (!exists)
            {
                await context.RolePermissions.AddAsync(RolePermission.Create(hrAdminRole.Id, p.Id));
            }
        }

        // Employee gets only read permissions for employees, departments, and joblevels
        foreach (var p in allDbPermissions.Where(per => per.PermissionCode == "hrm.employee.read" || per.PermissionCode == "hrm.department.read" || per.PermissionCode == "hrm.joblevel.read"))
        {
            var exists = await context.RolePermissions.AnyAsync(rp => rp.RoleId == employeeRole.Id && rp.PermissionId == p.Id);
            if (!exists)
            {
                await context.RolePermissions.AddAsync(RolePermission.Create(employeeRole.Id, p.Id));
            }
        }
        await context.SaveChangesAsync();

        // 6. Seed a Default Admin User & Account
        if (!await context.Users.AnyAsync(u => u.EmployeeCode == "ADMIN001"))
        {
            var adminUser = User.Create(
                "ADMIN001",
                "System Administrator",
                "admin@company.com",
                rootDept.Id,
                directorLevel.Id,
                new DateOnly(2026, 1, 1),
                UserStatus.Active
            );
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // PasswordHash for "AdminPassword123!"
            var pwdHash = PasswordHasher.Hash("AdminPassword123!");
            var adminAccount = UserAccount.Create(adminUser.Id, "admin@company.com", pwdHash);
            await context.UserAccounts.AddAsync(adminAccount);

            var adminDept = UserDepartment.Create(adminUser.Id, rootDept.Id, isPrimary: true, new DateOnly(2026, 1, 1));
            await context.UserDepartments.AddAsync(adminDept);

            var adminRole = UserRole.Create(adminUser.Id, superAdminRole.Id);
            await context.UserRoles.AddAsync(adminRole);

            // Set manager on root department
            rootDept.SetManager(adminUser.Id);
            context.Departments.Update(rootDept);

            await context.SaveChangesAsync();
        }
    }
}
