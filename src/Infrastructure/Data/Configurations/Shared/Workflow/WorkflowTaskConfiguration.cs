namespace Infrastructure;

public class WorkflowTaskConfiguration : AuditableEntityConfiguration<WorkflowTask, Guid>
{
    public override void Configure(EntityTypeBuilder<WorkflowTask> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkflowTasks");

        builder.Property(t => t.StepName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.StepOrder).IsRequired();
        builder.Property(t => t.AssignedTo).IsRequired();
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.Note).HasMaxLength(1000).IsRequired(false);
        builder.Property(t => t.ActedAt).IsRequired(false);

        builder.HasIndex(t => new { t.AssignedTo, t.Status });
    }
}
