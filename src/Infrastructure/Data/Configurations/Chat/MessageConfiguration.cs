namespace Infrastructure;

public class MessageConfiguration : SoftDeleteEntityConfiguration<Message, Guid>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.ToTable("Messages");

        builder.Property(m => m.Content).HasMaxLength(4000);
        builder.Property(m => m.MessageType).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ParentMessageID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
