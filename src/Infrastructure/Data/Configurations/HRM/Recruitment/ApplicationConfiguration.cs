namespace Infrastructure;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Application> b)
    {
        b.ToTable("Applications");
        b.HasKey(x => x.Id);

        b.Property(x => x.RejectionReason).HasMaxLength(1000);

        b.HasOne(x => x.Applicant)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.JobPosting)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.StageHistory)
            .WithOne(x => x.Application)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.InterviewSchedules)
            .WithOne(x => x.Application)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.ClientCascade);

        b.HasMany(x => x.Evaluations)
            .WithOne(x => x.Application)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
