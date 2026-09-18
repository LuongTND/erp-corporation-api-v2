namespace Infrastructure;

public class ApplicantConfiguration : AuditableEntityConfiguration<Applicant, Guid>
{
    public override void Configure(EntityTypeBuilder<Applicant> builder)
    {
        base.Configure(builder);

        builder.ToTable("Applicants");

        builder.Property(a => a.FullName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Email).HasMaxLength(256);
        builder.Property(a => a.Phone).HasMaxLength(20);
        builder.Property(a => a.Notes).HasMaxLength(2000);

        builder.Property(a => a.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(a => a.DeletedAt).IsRequired(false);
        builder.Property(a => a.DeletedBy).IsRequired(false);

        builder.HasIndex(a => a.Email).HasFilter("[Email] IS NOT NULL");
        builder.HasIndex(a => a.Phone).HasFilter("[Phone] IS NOT NULL");

        builder.HasMany(a => a.Documents)
            .WithOne(d => d.Applicant)
            .HasForeignKey(d => d.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
