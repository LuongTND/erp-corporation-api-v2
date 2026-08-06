namespace Infrastructure;

public class CustomFieldOptionConfiguration : BaseEntityConfiguration<CustomFieldOption, Guid>
{
    public override void Configure(EntityTypeBuilder<CustomFieldOption> builder)
    {
        base.Configure(builder);

        builder.ToTable("CustomFieldOptions");

        builder.Property(o => o.Value).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Label).IsRequired().HasMaxLength(200);
    }
}
