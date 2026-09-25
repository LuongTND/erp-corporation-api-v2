namespace Infrastructure;

public sealed class InterviewScheduleConfiguration : IEntityTypeConfiguration<InterviewSchedule>
{
    public void Configure(EntityTypeBuilder<InterviewSchedule> b)
    {
        b.ToTable("InterviewSchedules");
        b.HasKey(x => x.Id);

        b.Property(x => x.LocationNote).HasMaxLength(500);
        b.Property(x => x.Notes).HasMaxLength(2000);
        b.Property(x => x.InterviewResult).HasMaxLength(2000);

        b.HasOne(x => x.Application)
            .WithMany(x => x.InterviewSchedules)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.ClientCascade);

        b.HasOne(x => x.Interviewer)
            .WithMany()
            .HasForeignKey(x => x.InterviewerId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasMany(x => x.Evaluations)
            .WithOne(x => x.InterviewSchedule)
            .HasForeignKey(x => x.InterviewScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
