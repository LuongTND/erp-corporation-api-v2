namespace Application;

public sealed record BulkSyncRoleUsersCommand(
    Guid RoleId,
    IReadOnlyList<Guid> ToAdd,
    IReadOnlyList<Guid> ToRemove,
    DateTimeOffset? ExpiresAt
) : IRequest<Unit>;
