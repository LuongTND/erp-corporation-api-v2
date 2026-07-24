namespace Infrastructure;

public class TaskAssigneeConfiguration : BaseEntityConfiguration<TaskAssignee, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskAssignee> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskAssignees");

        builder.HasIndex(a => new { a.TaskID, a.UserID }).IsUnique();

        builder.HasOne(a => a.Task)
            .WithMany(t => t.Assignees)
            .HasForeignKey(a => a.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
