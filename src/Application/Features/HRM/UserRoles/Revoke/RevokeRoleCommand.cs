namespace Application;

public sealed record RevokeRoleCommand(Guid UserId, Guid RoleId) : IRequest<Unit>;
