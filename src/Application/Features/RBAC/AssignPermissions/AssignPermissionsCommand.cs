namespace Application;

public sealed record AssignPermissionsCommand(List<Guid> PermissionIds) : IRequest<Unit>
{
    public Guid RoleId { get; init; }
}
