using Application.Common.Models;

namespace Application.DTOs.JobLevels;

public class JobLevelDto : IHasGuidId
{
    public Guid Id { get; set; }
    public string LevelName { get; set; } = null!;
    public int LevelOrder { get; set; }
    public ScopeType DefaultScopeType { get; set; }
    public string? Description { get; set; }
    public decimal? BaseSalaryMin { get; set; }
    public decimal? BaseSalaryMax { get; set; }
    public bool IsActive { get; set; }
}
