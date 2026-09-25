namespace Infrastructure;

public sealed class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> b)
    {
        b.ToTable("JobPostings");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).HasMaxLength(300);
        b.Property(x => x.WorkLocation).HasMaxLength(300);
        b.Property(x => x.PostUrl).HasMaxLength(500);
        b.Property(x => x.Requirements).HasMaxLength(2000);

        b.HasOne(x => x.RecruitmentRequest)
            .WithMany(x => x.JobPostings)
            .HasForeignKey(x => x.RecruitmentRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.AssignedTo)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
