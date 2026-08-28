namespace Infrastructure;

public class InterviewScheduleConfiguration : AuditableEntityConfiguration<InterviewSchedule, Guid>
{
    public override void Configure(EntityTypeBuilder<InterviewSchedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterviewSchedules");

        builder.Property(s => s.Location)
            .HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(s => s.LocationNote).HasMaxLength(500);
        builder.Property(s => s.Notes).HasMaxLength(2000);
        builder.Property(s => s.InterviewResult).HasMaxLength(2000);

        builder.HasIndex(s => s.CandidateId);
        builder.HasIndex(s => new { s.InterviewerId, s.ScheduledAt });

        builder.HasOne(s => s.Candidate)
            .WithMany(c => c.InterviewSchedules)
            .HasForeignKey(s => s.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Interviewer)
            .WithMany()
            .HasForeignKey(s => s.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
