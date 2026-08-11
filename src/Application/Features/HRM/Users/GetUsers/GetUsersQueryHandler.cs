namespace Application;

public sealed class GetUsersQueryHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<GetUsersQuery, IEnumerable<UserSummaryResponse>>
{
    private const string Container = "avatars";

    public async Task<IEnumerable<UserSummaryResponse>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        List<Guid>? deptUserIds = null;
        if (query.DepartmentId.HasValue)
        {
            var members = await unitOfWork.Repository<UserDepartment>()
                .GetAllAsync(ud => ud.DepartmentId == query.DepartmentId.Value && ud.IsActive, ct);
            deptUserIds = members.Select(ud => ud.UserId).ToList();
            if (deptUserIds.Count == 0) return [];
        }

        var result = await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: u => (query.Status == null ? u.IsActive : u.Status == query.Status.Value)
                && (query.Search == null || u.FullName.Contains(query.Search) || u.EmployeeCode.Contains(query.Search))
                && (query.JobLevelId == null || u.JobLevelId == query.JobLevelId)
                && (deptUserIds == null || deptUserIds.Contains(u.Id)),
            orderBy: q => q.OrderBy(u => u.FullName),
            ct: ct);

        return result.Items.Select(u => new UserSummaryResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            EmployeeCode = u.EmployeeCode,
            Email = u.Email,
            AvatarUrl = u.AvatarUrl is null ? null : blobStorage.GetUrl(Container, u.AvatarUrl),
            Status = u.Status.ToString(),
            JoinDate = u.CreatedAt,
        });
    }
}
