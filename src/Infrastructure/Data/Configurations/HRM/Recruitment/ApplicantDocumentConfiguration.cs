namespace Infrastructure;

public class ApplicantDocumentConfiguration : AuditableEntityConfiguration<ApplicantDocument, Guid>
{
    public override void Configure(EntityTypeBuilder<ApplicantDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("ApplicantDocuments");

        builder.Property(d => d.DocumentType).HasMaxLength(100);

        builder.Property(d => d.FileName).HasMaxLength(500).IsRequired();
        builder.Property(d => d.FileUrl).HasMaxLength(1000).IsRequired();

        builder.HasIndex(d => d.ApplicantId);
    }
}
