namespace Application;

public sealed record SetScopeOverrideCommand(Guid UserId, ScopeType? ScopeOverride) : IRequest<Unit>;
