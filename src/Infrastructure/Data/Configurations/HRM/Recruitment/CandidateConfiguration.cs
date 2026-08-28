namespace Infrastructure;

public class CandidateConfiguration : AuditableEntityConfiguration<Candidate, Guid>
{
    public override void Configure(EntityTypeBuilder<Candidate> builder)
    {
        base.Configure(builder);

        builder.ToTable("Candidates");

        builder.Property(c => c.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.CvUrl)
            .HasMaxLength(1000);

        builder.Property(c => c.SourceChannel)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Stage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        // soft delete
        builder.Property(c => c.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(c => c.DeletedAt).IsRequired(false);
        builder.Property(c => c.DeletedBy).IsRequired(false);

        builder.HasIndex(c => new { c.RecruitmentRequestId, c.Stage });

        builder.HasMany(c => c.Evaluations)
            .WithOne(e => e.Candidate)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
