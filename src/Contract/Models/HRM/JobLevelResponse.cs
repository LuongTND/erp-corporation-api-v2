namespace Contract;

public sealed class JobLevelResponse
{
    public Guid Id { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int LevelOrder { get; set; }
    public string DefaultScopeType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public int EmployeeCount { get; set; }
}
