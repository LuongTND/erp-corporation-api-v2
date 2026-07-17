namespace Infrastructure;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.ToTable("Conversation_Members");

        builder.HasKey(x => new { x.ConversationID, x.UserID });

        builder.HasOne(x => x.Conversation)
            .WithMany(c => c.Members)
            .HasForeignKey(x => x.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LastReadMessage)
            .WithMany()
            .HasForeignKey(x => x.LastReadMessageID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RoleInConversation)
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(x => x.JoinedAt)
            .IsRequired();

        builder.Property(x => x.IsMuted)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}