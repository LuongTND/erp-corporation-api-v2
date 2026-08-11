namespace Domain;

public class KpiTemplate : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid? JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<KpiMetric> Metrics { get; set; } = [];
}
