namespace Infrastructure;

public class TaskPriorityConfiguration : BaseEntityConfiguration<TaskPriority, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskPriority> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskPriorities");

        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Code).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Color).IsRequired().HasMaxLength(20);
    }
}
