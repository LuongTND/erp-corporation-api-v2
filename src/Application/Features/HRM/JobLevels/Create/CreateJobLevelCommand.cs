namespace Application;

public sealed record CreateJobLevelCommand(
    string LevelName,
    int LevelOrder,
    ScopeType DefaultScopeType,
    string? Description
) : IRequest<Guid>;
