namespace Infrastructure;

public class JobPostingConfiguration : AuditableEntityConfiguration<JobPosting, Guid>
{
    public override void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobPostings");

        builder.Property(j => j.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(j => j.Channel)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(j => j.PostUrl)
            .HasMaxLength(1000);

        builder.Property(j => j.EstimatedCost)
            .HasPrecision(18, 2);

        builder.Property(j => j.CostStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(j => j.CostRejectionNote)
            .HasMaxLength(1000);

        builder.HasOne(j => j.CostApprovedBy)
            .WithMany()
            .HasForeignKey(j => j.CostApprovedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
