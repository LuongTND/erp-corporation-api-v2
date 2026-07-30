namespace Application;

public sealed record UpdateRoleCommand(
    string DisplayName,
    string? Description
) : IRequest<Unit>
{
    public Guid RoleId { get; init; }
}
