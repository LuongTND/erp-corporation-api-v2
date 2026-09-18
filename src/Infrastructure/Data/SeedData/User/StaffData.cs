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
        var JobTitleId = GuidHelper.From("Staff");
        if (!await context.Set<JobTitle>().AnyAsync(j => j.Id == JobTitleId))
        {
            context.Set<JobTitle>().Add(new JobTitle
            {
                Id = JobTitleId,
                Code = "STAFF",
                Name = "Staff",
                Level = JobTitleLevel.Staff,
                UnitType = JobTitleUnitType.Department,
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
