namespace Infrastructure;

public class InterviewRuleConfigStepConfiguration : BaseEntityConfiguration<InterviewRuleConfigStep, Guid>
{
    public override void Configure(EntityTypeBuilder<InterviewRuleConfigStep> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterviewRuleConfigSteps");

        builder.Property(s => s.Label).HasMaxLength(200).IsRequired();
        builder.Property(s => s.InterviewerRoleKey).HasMaxLength(200).IsRequired();
        builder.Property(s => s.SchedulerRoleKey).HasMaxLength(200).IsRequired();

        builder.Property(s => s.Location)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // mỗi config chỉ có 1 step per round
        builder.HasIndex(s => new { s.InterviewRuleConfigId, s.RoundNumber }).IsUnique();
    }
}
