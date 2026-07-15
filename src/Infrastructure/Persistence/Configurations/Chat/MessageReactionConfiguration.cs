namespace Infrastructure;

public class MessageReactionConfiguration : IEntityTypeConfiguration<MessageReaction>
{
    public void Configure(EntityTypeBuilder<MessageReaction> builder)
    {
        builder.ToTable("Message_Reactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ReactionID");

        builder.Property(x => x.ReactionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Message)
            .WithMany(m => m.Reactions)
            .HasForeignKey(x => x.MessageID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        // A user can react with one specific reaction type once per message
        builder.HasIndex(x => new { x.MessageID, x.UserID, x.ReactionType })
            .IsUnique();
    }
}