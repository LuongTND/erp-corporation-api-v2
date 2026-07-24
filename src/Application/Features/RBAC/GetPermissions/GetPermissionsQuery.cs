namespace Application;

public sealed record GetPermissionsQuery : IRequest<IEnumerable<PermissionResponse>>;
