namespace Infrastructure;

public class JobLevelConfiguration : AuditableEntityConfiguration<JobLevel, Guid>
{
    public override void Configure(EntityTypeBuilder<JobLevel> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobLevels");

        builder.Property(j => j.LevelName).IsRequired().HasMaxLength(100);
        builder.Property(j => j.Description).HasMaxLength(500);
    }
}
