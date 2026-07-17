namespace Infrastructure;

public class TaskFollowerConfiguration : IEntityTypeConfiguration<TaskFollower>
{
    public void Configure(EntityTypeBuilder<TaskFollower> builder)
    {
        builder.ToTable("Task_Followers");

        builder.HasKey(x => new { x.TaskID, x.UserID });

        builder.HasOne(x => x.Task)
            .WithMany(t => t.Followers)
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FollowedAt)
            .IsRequired();
    }
}