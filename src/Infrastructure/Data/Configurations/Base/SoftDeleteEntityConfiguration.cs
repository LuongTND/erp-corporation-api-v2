namespace Infrastructure;

public abstract class SoftDeleteEntityConfiguration<TEntity, TKey>
    : BaseEntityConfiguration<TEntity, TKey>
    where TKey : notnull
    where TEntity : EntityBase<TKey>, ISoftDeletable
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.DeletedAt).IsRequired(false);
        builder.Property(x => x.DeletedBy).IsRequired(false);
    }
}