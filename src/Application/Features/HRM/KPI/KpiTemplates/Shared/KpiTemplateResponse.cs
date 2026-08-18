namespace Application;

public sealed class KpiTemplateResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public Guid? JobLevelId { get; set; }
    public string? JobLevelName { get; set; }
    public bool IsActive { get; set; }
    public List<KpiMetricResponse> Metrics { get; set; } = [];
}
