namespace Infrastructure;

public class TaskFollowerConfiguration : BaseEntityConfiguration<TaskFollower, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskFollower> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskFollowers");

        builder.HasIndex(f => new { f.TaskID, f.UserID }).IsUnique();

        builder.HasOne(f => f.Task)
            .WithMany(t => t.Followers)
            .HasForeignKey(f => f.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
