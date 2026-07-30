namespace Application;

public sealed record DeleteRoleCommand(Guid RoleId) : IRequest<Unit>;
