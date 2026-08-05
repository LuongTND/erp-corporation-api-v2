namespace Application;

public sealed record UpdateJobLevelCommand(
    Guid JobLevelId,
    string LevelName,
    int LevelOrder,
    ScopeType DefaultScopeType,
    string? Description,
    decimal? BaseSalaryMin,
    decimal? BaseSalaryMax
) : IRequest<Unit>;
