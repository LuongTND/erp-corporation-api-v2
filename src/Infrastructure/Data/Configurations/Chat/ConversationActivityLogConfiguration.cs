namespace Infrastructure;

public class ConversationActivityLogConfiguration : IEntityTypeConfiguration<ConversationActivityLog>
{
    public void Configure(EntityTypeBuilder<ConversationActivityLog> builder)
    {
        builder.ToTable("Conversation_Activity_Log");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("LogID");

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.Conversation)
            .WithMany(c => c.ActivityLogs)
            .HasForeignKey(x => x.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}