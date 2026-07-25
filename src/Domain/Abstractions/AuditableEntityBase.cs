namespace Domain;

public abstract class AuditableEntityBase<T> : EntityBase<T>, IAuditable where T : notnull
{
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
