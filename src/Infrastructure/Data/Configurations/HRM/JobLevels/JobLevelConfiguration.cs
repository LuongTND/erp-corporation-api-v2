namespace Infrastructure;

public class JobLevelConfiguration : AuditableEntityConfiguration<JobLevel, Guid>
{
    public override void Configure(EntityTypeBuilder<JobLevel> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobLevels");

        builder.Property(j => j.LevelName).IsRequired().HasMaxLength(100);
        builder.Property(j => j.DefaultScopeType).HasConversion<string>().HasMaxLength(30);
        builder.Property(j => j.Description).HasMaxLength(500);
        builder.Property(j => j.BaseSalaryMin).HasPrecision(18, 2);
        builder.Property(j => j.BaseSalaryMax).HasPrecision(18, 2);
    }
}
