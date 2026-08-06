namespace Application;

public sealed class JobLevelResponse
{
    public Guid Id { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int LevelOrder { get; set; }
    public ScopeType DefaultScopeType { get; set; }
    public string? Description { get; set; }
    public decimal? BaseSalaryMin { get; set; }
    public decimal? BaseSalaryMax { get; set; }
    public bool IsDeleted { get; set; }
}
