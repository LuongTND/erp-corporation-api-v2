namespace Infrastructure;

public class CandidateEvaluationConfiguration : AuditableEntityConfiguration<CandidateEvaluation, Guid>
{
    public override void Configure(EntityTypeBuilder<CandidateEvaluation> builder)
    {
        base.Configure(builder);

        builder.ToTable("CandidateEvaluations");

        builder.Property(e => e.Score)
            .IsRequired();

        builder.Property(e => e.IsStoreEvaluation)
            .IsRequired();

        builder.Property(e => e.Recommendation)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.StrengthNotes)
            .HasMaxLength(2000);

        builder.Property(e => e.WeaknessNotes)
            .HasMaxLength(2000);

        // 1 evaluator chỉ submit 1 lần / candidate
        builder.HasIndex(e => new { e.CandidateId, e.EvaluatorId }).IsUnique();

        builder.HasOne(e => e.Evaluator)
            .WithMany()
            .HasForeignKey(e => e.EvaluatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
