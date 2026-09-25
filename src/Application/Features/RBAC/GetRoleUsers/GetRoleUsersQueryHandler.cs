namespace Application;

public sealed class GetRoleUsersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRoleUsersQuery, IEnumerable<UserSummaryResponse>>
{
    public async Task<IEnumerable<UserSummaryResponse>> Handle(GetRoleUsersQuery query, CancellationToken ct) =>
        await unitOfWork.Repository<UserRole>().Query()
            .Where(ur => ur.RoleId == query.RoleId && ur.IsActive && ur.RevokedAt == null)
            .Join(unitOfWork.Repository<User>().Query().Where(u => u.IsActive),
                ur => ur.UserId,
                u => u.Id,
                (_, u) => u)
            .OrderBy(u => u.FullName)
            .ProjectToType<UserSummaryResponse>()
            .ToListAsync(ct);
}
