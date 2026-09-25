namespace Infrastructure;

public class InterviewRuleConfigConfiguration : AuditableEntityConfiguration<InterviewRuleConfig, Guid>
{
    public override void Configure(EntityTypeBuilder<InterviewRuleConfig> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterviewRuleConfigs");

        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();

        builder.Property(r => r.Context)
            .HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(r => r.Location)
            .HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(r => r.InterviewerRoleKey).HasMaxLength(200).IsRequired();
        builder.Property(r => r.SchedulerRoleKey).HasMaxLength(200).IsRequired();
        builder.Property(r => r.NotifyRoleKey).HasMaxLength(200).IsRequired();

        builder.Property(r => r.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(r => new { r.Context, r.RegionId, r.IsActive });

        builder.HasOne(r => r.Region)
            .WithMany()
            .HasForeignKey(r => r.RegionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Department)
            .WithMany()
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
