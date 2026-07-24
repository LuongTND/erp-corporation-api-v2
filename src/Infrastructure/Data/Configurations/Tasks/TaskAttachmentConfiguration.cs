namespace Infrastructure;

public class TaskAttachmentConfiguration : BaseEntityConfiguration<TaskAttachment, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskAttachments");

        builder.Property(a => a.Name).IsRequired().HasMaxLength(500);
        builder.Property(a => a.MimeType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.DataUrl).IsRequired();

        builder.HasOne(a => a.Task)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
