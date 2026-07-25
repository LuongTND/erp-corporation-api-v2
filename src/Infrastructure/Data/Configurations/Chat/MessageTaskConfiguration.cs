namespace Infrastructure;

public class MessageTaskConfiguration : BaseEntityConfiguration<MessageTask, Guid>
{
    public override void Configure(EntityTypeBuilder<MessageTask> builder)
    {
        base.Configure(builder);

        builder.ToTable("MessageTasks");

        builder.HasIndex(mt => new { mt.MessageID, mt.TaskID }).IsUnique();

        builder.HasOne(mt => mt.Message)
            .WithMany(m => m.MessageTasks)
            .HasForeignKey(mt => mt.MessageID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mt => mt.Task)
            .WithMany()
            .HasForeignKey(mt => mt.TaskID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
