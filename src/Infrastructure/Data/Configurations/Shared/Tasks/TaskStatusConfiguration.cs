namespace Infrastructure;

public class TaskItemStatusConfiguration : BaseEntityConfiguration<TaskItemStatus, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskItemStatus> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskStatuses");

        builder.HasIndex(s => s.Code).IsUnique();

        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Color).IsRequired().HasMaxLength(20);
    }
}
