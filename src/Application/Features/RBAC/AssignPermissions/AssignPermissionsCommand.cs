namespace Application;

public sealed record AssignPermissionsCommand(List<Guid> ToAdd, List<Guid> ToRemove) : IRequest<Unit>
{
    public Guid RoleId { get; init; }
}
