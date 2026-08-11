namespace Domain;

public class DepartmentJobLevel : AuditableEntityBase<Guid>, ISoftDeletable
{
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid JobLevelId { get; set; }
    public JobLevel JobLevel { get; set; } = null!;

    public Guid? BonusPolicyId { get; set; }
    public BonusPolicy? BonusPolicy { get; set; }

    public Guid? KpiTemplateId { get; set; }
    public KpiTemplate? KpiTemplate { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
