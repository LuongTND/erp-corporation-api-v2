namespace Application;

public sealed record CreateRoleCommand(
    string RoleName,
    string DisplayName,
    string? Description,
    ScopeType DefaultDataScope = ScopeType.Own
) : IRequest<Guid>;
