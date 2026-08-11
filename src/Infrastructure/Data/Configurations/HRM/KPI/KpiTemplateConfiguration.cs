namespace Infrastructure;

public class KpiTemplateConfiguration : AuditableEntityConfiguration<KpiTemplate, Guid>
{
    public override void Configure(EntityTypeBuilder<KpiTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("KpiTemplates");

        builder.Property(t => t.Name).IsRequired().HasMaxLength(255);

        builder.HasIndex(t => new { t.DepartmentId, t.JobLevelId }).IsUnique();

        builder.HasOne(t => t.Department)
            .WithMany(d => d.KpiTemplates)
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.JobLevel)
            .WithMany()
            .HasForeignKey(t => t.JobLevelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
