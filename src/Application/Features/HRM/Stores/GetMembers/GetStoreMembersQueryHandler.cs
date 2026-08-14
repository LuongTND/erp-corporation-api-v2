namespace Application;

public sealed class GetStoreMembersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetStoreMembersQuery, IEnumerable<StoreMemberResponse>>
{
    public async Task<IEnumerable<StoreMemberResponse>> Handle(GetStoreMembersQuery query, CancellationToken ct)
    {
        var memberships = (await unitOfWork.Repository<UserStore>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: us => us.StoreId == query.StoreId && us.IsActive,
            ct: ct)).Items.ToList();

        if (memberships.Count == 0) return [];

        var userIds = memberships.Select(us => us.UserId).Distinct().ToList();
        var users = (await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = userIds.Count, NeedTotalCount = false },
            filter: u => userIds.Contains(u.Id),
            ct: ct)).Items.ToDictionary(u => u.Id);

        var levelIds = users.Values.Where(u => u.JobLevelId.HasValue)
            .Select(u => u.JobLevelId!.Value).Distinct().ToList();
        var jobLevels = levelIds.Count > 0
            ? (await unitOfWork.Repository<JobLevel>().GetPagedAsync(
                new QueryInfo { Top = levelIds.Count, NeedTotalCount = false },
                filter: jl => levelIds.Contains(jl.Id),
                ct: ct)).Items.ToDictionary(jl => jl.Id)
            : new Dictionary<Guid, JobLevel>();

        return memberships
            .Where(us => users.ContainsKey(us.UserId))
            .Select(us =>
            {
                var user = users[us.UserId];
                jobLevels.TryGetValue(user.JobLevelId ?? Guid.Empty, out var level);
                return new StoreMemberResponse
                {
                    UserStoreId = us.Id,
                    UserId = us.UserId,
                    FullName = user.FullName,
                    EmployeeCode = user.EmployeeCode,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                    JobLevelName = level?.LevelName,
                    IsHomeStore = us.IsHomeStore,
                    StartDate = us.StartDate,
                };
            })
            .OrderBy(m => m.FullName);
    }
}
