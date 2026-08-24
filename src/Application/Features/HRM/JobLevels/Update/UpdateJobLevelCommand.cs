namespace Application;

public sealed record UpdateJobLevelCommand(
    Guid JobLevelId,
    string LevelName,
    int LevelOrder,
    string? Description
) : IRequest<Unit>;
