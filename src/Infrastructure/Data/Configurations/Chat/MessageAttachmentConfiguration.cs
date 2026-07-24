namespace Infrastructure;

public class MessageAttachmentConfiguration : BaseEntityConfiguration<MessageAttachment, Guid>
{
    public override void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("MessageAttachments");

        builder.Property(a => a.FileName).IsRequired().HasMaxLength(500);
        builder.Property(a => a.FileURL).IsRequired().HasMaxLength(2000);
        builder.Property(a => a.FileType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.ThumbnailURL).HasMaxLength(2000);

        builder.HasOne(a => a.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(a => a.MessageID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
