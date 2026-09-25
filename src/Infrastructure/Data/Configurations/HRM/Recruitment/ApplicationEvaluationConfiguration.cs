namespace Infrastructure;

public class ApplicationEvaluationConfiguration : AuditableEntityConfiguration<ApplicationEvaluation, Guid>
{
    public override void Configure(EntityTypeBuilder<ApplicationEvaluation> builder)
    {
        base.Configure(builder);

        builder.ToTable("ApplicationEvaluations");

        builder.Property(e => e.Score).IsRequired();

        builder.Property(e => e.Recommendation)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.StrengthNotes).HasMaxLength(2000);
        builder.Property(e => e.WeaknessNotes).HasMaxLength(2000);

        // 1 evaluator chỉ submit 1 lần per schedule (hoặc per application nếu không có schedule)
        builder.HasIndex(e => new { e.ApplicationId, e.EvaluatorId, e.InterviewScheduleId }).IsUnique();

        builder.HasOne(e => e.Evaluator)
            .WithMany()
            .HasForeignKey(e => e.EvaluatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InterviewSchedule)
            .WithMany()
            .HasForeignKey(e => e.InterviewScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
