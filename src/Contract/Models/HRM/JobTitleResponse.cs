namespace Contract;

public sealed class JobTitleResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Level { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public int EmployeeCount { get; set; }
}
