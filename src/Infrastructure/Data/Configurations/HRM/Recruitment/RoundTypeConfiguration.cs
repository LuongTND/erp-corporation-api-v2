namespace Infrastructure;

public class RoundTypeConfiguration : BaseEntityConfiguration<RoundType, Guid>
{
    public override void Configure(EntityTypeBuilder<RoundType> builder)
    {
        base.Configure(builder);

        builder.ToTable("RoundTypes");

        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();
    }
}
