namespace Application;

public sealed record DeletePermissionCommand(Guid PermissionId) : IRequest<Unit>;
