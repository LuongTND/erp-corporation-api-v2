namespace Application;

public sealed class GetRoleUsersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRoleUsersQuery, IEnumerable<UserSummaryResponse>>
{
    public async Task<IEnumerable<UserSummaryResponse>> Handle(GetRoleUsersQuery query, CancellationToken ct)
    {
        var userRoles = await unitOfWork.Repository<UserRole>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: ur => ur.RoleId == query.RoleId && ur.IsActive && ur.RevokedAt == null,
            ct: ct);

        if (!userRoles.Items.Any()) return [];

        var userIds = userRoles.Items.Select(ur => ur.UserId).Distinct().ToList();

        var users = await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = userIds.Count, NeedTotalCount = false },
            filter: u => userIds.Contains(u.Id) && u.IsActive,
            orderBy: q => q.OrderBy(u => u.FullName),
            ct: ct);

        return users.Items.Adapt<IEnumerable<UserSummaryResponse>>();
    }
}
