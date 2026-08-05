namespace Infrastructure;

public class TaskKpiConfiguration : BaseEntityConfiguration<TaskKpi, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskKpi> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskKpis");

        builder.HasIndex(k => new { k.TaskID, k.KpiId }).IsUnique();

        builder.Property(k => k.Weight).HasPrecision(5, 4);

        builder.HasOne(k => k.Task)
            .WithMany(t => t.TaskKpis)
            .HasForeignKey(k => k.TaskID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
