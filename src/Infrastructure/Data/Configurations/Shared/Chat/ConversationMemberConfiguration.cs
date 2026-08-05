namespace Infrastructure;

public class ConversationMemberConfiguration : BaseEntityConfiguration<ConversationMember, Guid>
{
    public override void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConversationMembers");

        builder.HasIndex(m => new { m.ConversationID, m.UserID }).IsUnique();

        builder.Property(m => m.RoleInConversation).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserID)
            .OnDelete(DeleteBehavior.NoAction);

        // NoAction: avoid multiple cascade paths (Conversation → Message AND Conversation → Member → Message)
        builder.HasOne(m => m.LastReadMessage)
            .WithMany()
            .HasForeignKey(m => m.LastReadMessageID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
