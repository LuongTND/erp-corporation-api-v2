namespace Infrastructure;

public class JobTitleConfiguration : AuditableEntityConfiguration<JobTitle, Guid>
{
    public override void Configure(EntityTypeBuilder<JobTitle> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobTitles");

        builder.Property(j => j.Code).IsRequired().HasMaxLength(50);
        builder.Property(j => j.Name).IsRequired().HasMaxLength(100);
        builder.Property(j => j.Description).HasMaxLength(500);
    }
}
