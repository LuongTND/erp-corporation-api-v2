namespace Infrastructure;

public class ContractTemplateConfiguration : AuditableEntityConfiguration<ContractTemplate, Guid>
{
    public override void Configure(EntityTypeBuilder<ContractTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("ContractTemplates");

        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.BlobName).HasMaxLength(500).IsRequired();
        builder.Property(t => t.OriginalFileName).HasMaxLength(255).IsRequired();
        builder.HasMany(t => t.Contracts)
            .WithOne(c => c.Template)
            .HasForeignKey(c => c.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
