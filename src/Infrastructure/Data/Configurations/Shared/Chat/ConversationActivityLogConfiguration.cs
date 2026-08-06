namespace Infrastructure;

public class ConversationActivityLogConfiguration : BaseEntityConfiguration<ConversationActivityLog, Guid>
{
    public override void Configure(EntityTypeBuilder<ConversationActivityLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConversationActivityLogs");

        builder.Property(l => l.Action).HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.Description).HasMaxLength(1000);

        builder.HasOne(l => l.Conversation)
            .WithMany(c => c.ActivityLogs)
            .HasForeignKey(l => l.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
