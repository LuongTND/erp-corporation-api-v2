namespace Infrastructure;

public abstract class AuditableEntityConfiguration<TEntity, TKey>
    : BaseEntityConfiguration<TEntity, TKey>
    where TKey : notnull
    where TEntity : AuditableEntityBase<TKey>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CreatedBy).IsRequired(false);
        builder.Property(e => e.UpdatedBy).IsRequired(false);
    }
}
