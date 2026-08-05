namespace Infrastructure;

public class TaskDependencyConfiguration : BaseEntityConfiguration<TaskDependency, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskDependencies");

        builder.HasIndex(d => new { d.BlockerTaskId, d.BlockedTaskId }).IsUnique();

        builder.Property(d => d.DependencyType).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(d => d.BlockerTask)
            .WithMany(t => t.BlockingTasks)
            .HasForeignKey(d => d.BlockerTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.BlockedTask)
            .WithMany(t => t.BlockedByTasks)
            .HasForeignKey(d => d.BlockedTaskId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
