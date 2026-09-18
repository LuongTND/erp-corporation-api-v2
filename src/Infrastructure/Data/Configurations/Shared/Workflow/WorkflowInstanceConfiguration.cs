namespace Infrastructure;

public class WorkflowInstanceConfiguration : AuditableEntityConfiguration<WorkflowInstance, Guid>
{
    public override void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkflowInstances");

        builder.Property(i => i.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(i => i.EntityId).IsRequired();
        builder.Property(i => i.CurrentStep).IsRequired();
        builder.Property(i => i.Status).IsRequired();
        builder.Property(i => i.CompletedAt).IsRequired(false);

        builder.HasIndex(i => new { i.EntityType, i.EntityId });

        builder.HasOne(i => i.Template)
            .WithMany()
            .HasForeignKey(i => i.TemplateId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(i => i.Tasks)
            .WithOne(t => t.Instance)
            .HasForeignKey(t => t.InstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
