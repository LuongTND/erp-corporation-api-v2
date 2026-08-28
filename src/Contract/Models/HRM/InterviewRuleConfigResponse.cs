namespace Contract;

public class InterviewRuleConfigResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Context { get; init; } = string.Empty;
    public Guid? RegionId { get; init; }
    public string? RegionName { get; init; }
    public Guid? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public string InterviewerRoleKey { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string SchedulerRoleKey { get; init; } = string.Empty;
    public string NotifyRoleKey { get; init; } = string.Empty;
    public int Priority { get; init; }
    public bool IsActive { get; init; }
}
