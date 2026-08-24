namespace Application;

public sealed record CreateJobLevelCommand(
    string LevelName,
    int LevelOrder,
    string? Description
) : IRequest<Guid>;
