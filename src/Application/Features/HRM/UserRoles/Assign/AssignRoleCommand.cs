namespace Application;

public sealed record AssignRoleCommand(
    Guid UserId,
    Guid RoleId,
    DateTimeOffset? ExpiresAt
) : IRequest<Guid>;
