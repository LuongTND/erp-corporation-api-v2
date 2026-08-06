namespace Infrastructure;

public static class StaffData
{
    private static readonly string[] Usernames =
    [
        "SangTQ", "PhuongHTK",
        "LuongTND", "HungDNB", "HuyTQ", "HungNDM",
        "HaNTC", "HuongLTT", "TrangNTT",
        "HanhTTH", "TrangVTH", "TuyetTHA",
        "HungPT",
        "DaiLT", "ThaiTD",
        "NgocNTH", "PhuongVT",
    ];

    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        var jobLevelId = GuidHelper.From("Staff");
        if (!await context.Set<JobLevel>().AnyAsync(j => j.Id == jobLevelId))
        {
            context.Set<JobLevel>().Add(new JobLevel
            {
                Id = jobLevelId,
                LevelName = "Staff",
                LevelOrder = 1,
                DefaultScopeType = ScopeType.Department,
            });
            await context.SaveChangesAsync();
        }

        foreach (var username in Usernames)
        {
            var email = $"{username}@gmail.com";
            if (await context.Set<UserAccount>().AnyAsync(a => a.LoginEmail == email))
                continue;

            var userId = GuidHelper.From(email);
            var user = new User
            {
                Id = userId,
                EmployeeCode = username.ToUpper(),
                FullName = username,
                Email = email,
                JobLevelId = jobLevelId,
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
                Id = GuidHelper.From(email + "_account"),
                UserId = userId,
                LoginEmail = email,
                PasswordHash = hasher.Hash("!Abc123"),
                EmailVerified = true,
            });
        }

        await context.SaveChangesAsync();
    }
}
