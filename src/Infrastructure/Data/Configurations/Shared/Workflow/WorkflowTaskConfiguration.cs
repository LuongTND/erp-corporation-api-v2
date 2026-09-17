namespace Infrastructure;

public class WorkflowTaskConfiguration : AuditableEntityConfiguration<WorkflowTask, Guid>
{
    public override void Configure(EntityTypeBuilder<WorkflowTask> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkflowTasks");

        builder.Property(t => t.StepName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.StepOrder).IsRequired();
        builder.Property(t => t.AssignedTo).IsRequired(false);
        builder.Property(t => t.AssignedToRoleId).IsRequired(false);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.Note).HasMaxLength(1000).IsRequired(false);
        builder.Property(t => t.ActedAt).IsRequired(false);
        builder.Property(t => t.ActedByUserId).IsRequired(false);

        // index cho cả 2 lookup pattern: by user và by role
        builder.Property(t => t.RowVersion).IsRowVersion();

        builder.HasIndex(t => new { t.AssignedTo, t.Status });
        builder.HasIndex(t => new { t.AssignedToRoleId, t.Status });
    }
}
