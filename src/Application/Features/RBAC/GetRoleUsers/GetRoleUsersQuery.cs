namespace Application;

public sealed record GetRoleUsersQuery(Guid RoleId) : IRequest<IEnumerable<UserSummaryResponse>>;
