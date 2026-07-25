namespace Application;

public sealed record GetRolesQuery : IRequest<IEnumerable<RoleResponse>>;
