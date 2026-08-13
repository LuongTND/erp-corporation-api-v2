namespace Domain;

public class Counter : AuditableEntityBase<Guid>, ISoftDeletable
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
