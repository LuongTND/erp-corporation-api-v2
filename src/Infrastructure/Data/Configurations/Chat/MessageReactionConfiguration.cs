namespace Infrastructure;

public class MessageReactionConfiguration : BaseEntityConfiguration<MessageReaction, Guid>
{
    public override void Configure(EntityTypeBuilder<MessageReaction> builder)
    {
        base.Configure(builder);

        builder.ToTable("MessageReactions");

        builder.HasIndex(r => new { r.MessageID, r.UserID, r.ReactionType }).IsUnique();

        builder.Property(r => r.ReactionType).IsRequired().HasMaxLength(50);

        builder.HasOne(r => r.Message)
            .WithMany(m => m.Reactions)
            .HasForeignKey(r => r.MessageID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
