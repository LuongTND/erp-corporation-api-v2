namespace Application;

public sealed record CreateRoleCommand(
    string RoleName,
    string DisplayName,
    string? Description
) : IRequest<Guid>;
