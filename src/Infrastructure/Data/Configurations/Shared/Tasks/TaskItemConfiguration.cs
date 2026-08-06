namespace Infrastructure;

public class TaskItemConfiguration : AuditableEntityConfiguration<TaskItem, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskItems");

        builder.HasIndex(t => t.TaskCode).IsUnique();

        builder.Property(t => t.TaskCode).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Description).HasMaxLength(4000);
        builder.Property(t => t.TaskType).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.RecurringPattern).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.EstimatedHours).HasPrecision(10, 2);
        builder.Property(t => t.ActualHours).HasPrecision(10, 2);

        builder.HasOne(t => t.Status)
            .WithMany(s => s.Tasks)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Priority)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.PriorityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.Subtasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
