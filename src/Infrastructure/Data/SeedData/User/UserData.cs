namespace Infrastructure;

public static class UserData
{
    public static async Task SeedAdminAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        const string adminEmail = "admin@gmail.com";
        if (await context.Set<UserAccount>().AnyAsync(a => a.LoginEmail == adminEmail))
            return;

        var JobTitleId = GuidHelper.From("Admin");
        if (!await context.Set<JobTitle>().AnyAsync(j => j.Id == JobTitleId))
        {
            context.Set<JobTitle>().Add(new JobTitle
            {
                Id = JobTitleId,
                Code = "ADMIN",
                Name = "Admin",
                Level = JobTitleLevel.Leadership,
                UnitType = JobTitleUnitType.Company,
            });
            await context.SaveChangesAsync();
        }

        var userId = GuidHelper.From(adminEmail);
        var user = new User
        {
            Id = userId,
            EmployeeCode = "ADMIN001",
            FullName = "System Admin",
            Email = adminEmail,
            JobTitleId = JobTitleId,
        };
        user.ChangeStatus(UserStatus.Active);
        context.Set<User>().Add(user);

        context.Set<EmploymentInfo>().Add(new EmploymentInfo
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DateOfJoin = DateOnly.FromDateTime(DateTime.UtcNow),
        });

        context.Set<UserAccount>().Add(new UserAccount
        {
            Id = GuidHelper.From(adminEmail + "_account"),
            UserId = userId,
            LoginEmail = adminEmail,
            PasswordHash = hasher.Hash("!Abc123"),
            EmailVerified = true
        });

        var adminRole = await context.Roles.FirstAsync(r => r.RoleName == RoleConstants.Admin);
        context.Set<UserRole>().Add(new UserRole { UserId = userId, RoleId = adminRole.Id });

        await context.SaveChangesAsync();
    }
}
