using Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Chat;

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("Message_Attachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("AttachmentID");

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.FileURL)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.FileType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.ThumbnailURL)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(x => x.MessageID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
