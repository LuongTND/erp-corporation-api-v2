namespace Infrastructure;

public class ApplicantConfiguration : AuditableEntityConfiguration<Applicant, Guid>
{
    public override void Configure(EntityTypeBuilder<Applicant> builder)
    {
        base.Configure(builder);

        builder.ToTable("Applicants");

        builder.Property(a => a.FullName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Province).HasMaxLength(100);
        builder.Property(a => a.Ward).HasMaxLength(100);
        builder.Property(a => a.Address).HasMaxLength(500);
        builder.Property(a => a.Notes).HasMaxLength(2000);

        builder.Property(a => a.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(a => a.DeletedAt).IsRequired(false);
        builder.Property(a => a.DeletedBy).IsRequired(false);

        builder.HasMany(a => a.Phones)
            .WithOne(p => p.Applicant)
            .HasForeignKey(p => p.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Emails)
            .WithOne(e => e.Applicant)
            .HasForeignKey(e => e.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Educations)
            .WithOne(e => e.Applicant)
            .HasForeignKey(e => e.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Experiences)
            .WithOne(e => e.Applicant)
            .HasForeignKey(e => e.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Documents)
            .WithOne(d => d.Applicant)
            .HasForeignKey(d => d.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
