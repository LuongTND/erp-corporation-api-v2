namespace Application;

public sealed record UpdateUserStatusCommand(
    Guid UserId,
    UserStatus NewStatus,
    string? Note
) : IRequest<Unit>;
