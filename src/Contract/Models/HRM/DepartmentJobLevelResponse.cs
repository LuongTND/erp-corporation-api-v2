namespace Contract;

public sealed class DepartmentJobLevelResponse
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public Guid JobLevelId { get; set; }
    public string JobLevelName { get; set; } = string.Empty;
    public Guid? BonusPolicyId { get; set; }
    public string? BonusPolicyName { get; set; }
    public Guid? KpiTemplateId { get; set; }
    public string? KpiTemplateName { get; set; }
}
