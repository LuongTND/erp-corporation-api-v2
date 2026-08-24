namespace Infrastructure;

public class LabelConfiguration : BaseEntityConfiguration<Label, Guid>
{
    public override void Configure(EntityTypeBuilder<Label> builder)
    {
        base.Configure(builder);
        builder.ToTable("Labels");
        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Color).HasMaxLength(20).IsRequired();
        builder.HasIndex(l => l.Name).IsUnique();
    }
}
