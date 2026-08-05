namespace Application;

public sealed record CreateJobLevelCommand(
    string LevelName,
    int LevelOrder,
    ScopeType DefaultScopeType,
    string? Description,
    decimal? BaseSalaryMin,
    decimal? BaseSalaryMax
) : IRequest<Guid>;
