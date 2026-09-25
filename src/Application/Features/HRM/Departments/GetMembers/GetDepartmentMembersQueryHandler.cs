namespace Application;

public sealed class GetDepartmentMembersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepartmentMembersQuery, IEnumerable<DepartmentMemberResponse>>
{
    public async Task<IEnumerable<DepartmentMemberResponse>> Handle(GetDepartmentMembersQuery query, CancellationToken ct)
    {
        var memberships = (await unitOfWork.Repository<UserDepartment>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: ud => ud.DepartmentId == query.DepartmentId && ud.IsActive,
            ct: ct)).Items.ToList();

        if (memberships.Count == 0) return [];

        var userIds = memberships.Select(ud => ud.UserId).Distinct().ToList();

        var users = (await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = userIds.Count, NeedTotalCount = false },
            filter: u => userIds.Contains(u.Id),
            ct: ct)).Items.ToDictionary(u => u.Id);

        var allLevelIds = users.Values
            .Where(u => u.JobTitleId.HasValue)
            .Select(u => u.JobTitleId!.Value)
            .Distinct().ToList();

        var jobLevels = allLevelIds.Count > 0
            ? (await unitOfWork.Repository<JobTitle>().GetPagedAsync(
                new QueryInfo { Top = allLevelIds.Count, NeedTotalCount = false },
                filter: jl => allLevelIds.Contains(jl.Id),
                ct: ct)).Items.ToDictionary(jl => jl.Id)
            : new Dictionary<Guid, JobTitle>();

        return memberships
            .Where(ud => users.ContainsKey(ud.UserId))
            .Select(ud =>
            {
                var user = users[ud.UserId];
                var levelId = user.JobTitleId;
                jobLevels.TryGetValue(levelId ?? Guid.Empty, out var level);
                return new DepartmentMemberResponse
                {
                    UserDepartmentId = ud.Id,
                    UserId = ud.UserId,
                    FullName = user.FullName,
                    EmployeeCode = user.EmployeeCode,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                    JobTitleId = levelId == Guid.Empty ? null : levelId,
                    JobName = level?.Name,
                    IsPrimary = ud.IsPrimary,
                    StartDate = ud.StartDate
                };
            })
            .OrderBy(m => m.JobName ?? "zzz")
            .ThenBy(m => m.FullName);
    }
}
