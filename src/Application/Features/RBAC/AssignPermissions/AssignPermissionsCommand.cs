namespace Application;

public sealed record AssignPermissionsCommand(List<Guid> PermissionIds) : IRequest<MediatR.Unit>
{
    public Guid RoleId { get; init; }
}
