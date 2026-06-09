using Domain.Enums;

namespace Application.DTOs.JobLevels;

public record CreateJobLevelRequest(
    string LevelName,
    int LevelOrder,
    ScopeType DefaultScopeType,
    string? Description = null,
    decimal? BaseSalaryMin = null,
    decimal? BaseSalaryMax = null
);
