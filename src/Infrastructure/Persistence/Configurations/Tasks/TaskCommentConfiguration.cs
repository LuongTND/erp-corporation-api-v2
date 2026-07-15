namespace Infrastructure;

public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.ToTable("Task_Comments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("CommentID");

        builder.Property(x => x.Content)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.HasOne(x => x.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(x => x.ParentCommentID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}