namespace Infrastructure;

public class TaskCommentConfiguration : BaseEntityConfiguration<TaskComment, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskComments");

        builder.Property(c => c.Content).IsRequired().HasMaxLength(4000);

        builder.HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
