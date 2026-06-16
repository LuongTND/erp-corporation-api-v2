using Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Chat;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("MessageID");

        builder.Property(x => x.Content)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.MessageType)
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(x => x.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(x => x.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(x => x.ParentMessageID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsEdited)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();
    }
}
