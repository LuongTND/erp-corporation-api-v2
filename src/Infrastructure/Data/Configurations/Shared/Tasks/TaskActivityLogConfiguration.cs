namespace Infrastructure;

public class TaskActivityLogConfiguration : BaseEntityConfiguration<TaskActivityLog, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskActivityLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskActivityLogs");

        builder.Property(l => l.Action).HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.OldValue).HasMaxLength(1000);
        builder.Property(l => l.NewValue).HasMaxLength(1000);

        builder.HasOne(l => l.Task)
            .WithMany(t => t.ActivityLogs)
            .HasForeignKey(l => l.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
