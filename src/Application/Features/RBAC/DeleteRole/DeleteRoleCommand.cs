namespace Application;

public sealed record DeleteRoleCommand(Guid RoleId, bool Force = false) : IRequest<Unit>;
