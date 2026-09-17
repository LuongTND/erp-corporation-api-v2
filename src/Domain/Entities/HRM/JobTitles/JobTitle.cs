namespace Domain;

public class JobTitle : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobTitleLevel Level { get; set; }
    public JobTitleUnitType UnitType { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<User> Users { get; set; } = [];
}
