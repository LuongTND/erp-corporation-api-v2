namespace Application;

public sealed record UpdateRoleCommand(
    string DisplayName,
    string? Description,
    ScopeType DefaultDataScope = ScopeType.Own
) : IRequest<Unit>
{
    public Guid RoleId { get; init; }
}
