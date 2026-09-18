namespace Infrastructure;

public class ApplicationConfiguration : AuditableEntityConfiguration<Domain.Application, Guid>
{
    public override void Configure(EntityTypeBuilder<Domain.Application> builder)
    {
        base.Configure(builder);

        builder.ToTable("Applications");

        builder.Property(a => a.SourceChannel)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Stage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.RejectionReason).HasMaxLength(1000);

        builder.Property(a => a.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(a => a.DeletedAt).IsRequired(false);
        builder.Property(a => a.DeletedBy).IsRequired(false);

        builder.HasIndex(a => new { a.RecruitmentRequestId, a.Stage });
        builder.HasIndex(a => a.ApplicantId);
        builder.HasIndex(a => a.JobPostingId).HasFilter("[JobPostingId] IS NOT NULL");

        builder.HasOne(a => a.Applicant)
            .WithMany(ap => ap.Applications)
            .HasForeignKey(a => a.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.RecruitmentRequest)
            .WithMany(r => r.Applications)
            .HasForeignKey(a => a.RecruitmentRequestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.JobPosting)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobPostingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(a => a.Evaluations)
            .WithOne(e => e.Application)
            .HasForeignKey(e => e.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.InterviewSchedules)
            .WithOne(s => s.Application)
            .HasForeignKey(s => s.ApplicationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(a => a.StageHistory)
            .WithOne(h => h.Application)
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
