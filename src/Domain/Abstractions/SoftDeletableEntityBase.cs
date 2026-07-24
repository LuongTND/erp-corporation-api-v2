namespace Domain;

public abstract class SoftDeletableEntityBase<T> : EntityBase<T>, ISoftDeletable
    where T : notnull
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}