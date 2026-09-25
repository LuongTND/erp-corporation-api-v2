namespace Infrastructure;

public class CustomFieldDefinitionConfiguration : AuditableEntityConfiguration<CustomFieldDefinition, Guid>
{
    public override void Configure(EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        base.Configure(builder);

        builder.ToTable("CustomFieldDefinitions");

        builder.HasIndex(d => d.Code).IsUnique();

        builder.Property(d => d.Code).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.FieldType).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.Module).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Placeholder).HasMaxLength(200);
        builder.Property(d => d.HelpText).HasMaxLength(500);
        builder.Property(d => d.Group).HasMaxLength(100);
        builder.Property(d => d.ValidationJson).HasColumnType("nvarchar(max)");

        builder.HasMany(d => d.Options)
            .WithOne(o => o.Definition)
            .HasForeignKey(o => o.DefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
