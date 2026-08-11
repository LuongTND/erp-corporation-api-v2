namespace Infrastructure;

public class DepartmentJobLevelConfiguration : AuditableEntityConfiguration<DepartmentJobLevel, Guid>
{
    public override void Configure(EntityTypeBuilder<DepartmentJobLevel> builder)
    {
        base.Configure(builder);

        builder.ToTable("DepartmentJobLevels");

        builder.HasIndex(djl => new { djl.DepartmentId, djl.JobLevelId }).IsUnique();

        builder.HasOne(djl => djl.Department)
            .WithMany(d => d.DepartmentJobLevels)
            .HasForeignKey(djl => djl.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(djl => djl.JobLevel)
            .WithMany()
            .HasForeignKey(djl => djl.JobLevelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(djl => djl.BonusPolicy)
            .WithMany()
            .HasForeignKey(djl => djl.BonusPolicyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(djl => djl.KpiTemplate)
            .WithMany()
            .HasForeignKey(djl => djl.KpiTemplateId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
