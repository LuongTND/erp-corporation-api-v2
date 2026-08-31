namespace Infrastructure;

public class RecruitmentRequestConfiguration : AuditableEntityConfiguration<RecruitmentRequest, Guid>
{
    public override void Configure(EntityTypeBuilder<RecruitmentRequest> builder)
    {
        base.Configure(builder);

        builder.ToTable("RecruitmentRequests");

        builder.Property(r => r.RequestContext)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.RequestCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(r => r.RequestCode).IsUnique();

        builder.Property(r => r.PositionTitle)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Headcount)
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(r => r.JobDescription)
            .HasMaxLength(4000);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.RejectionNote)
            .HasMaxLength(1000);

        builder.Property(r => r.NeedMoreInfoNote)
            .HasMaxLength(1000);

        builder.Property(r => r.Level1Note)
            .HasMaxLength(1000);

        // soft delete
        builder.Property(r => r.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(r => r.DeletedAt).IsRequired(false);
        builder.Property(r => r.DeletedBy).IsRequired(false);

        // filter theo status + context nhanh
        builder.HasIndex(r => new { r.Status, r.RequestContext });
        builder.HasIndex(r => r.DepartmentId);
        builder.HasIndex(r => r.StoreId);

        builder.HasOne(r => r.Department)
            .WithMany()
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.RequestedBy)
            .WithMany()
            .HasForeignKey(r => r.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Level1Approver)
            .WithMany()
            .HasForeignKey(r => r.Level1ApproverId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.Level2Approver)
            .WithMany()
            .HasForeignKey(r => r.Level2ApproverId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(r => r.Candidates)
            .WithOne(c => c.RecruitmentRequest)
            .HasForeignKey(c => c.RecruitmentRequestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(r => r.JobPostings)
            .WithOne(j => j.RecruitmentRequest)
            .HasForeignKey(j => j.RecruitmentRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
