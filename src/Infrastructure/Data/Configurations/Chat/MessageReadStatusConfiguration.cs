namespace Infrastructure;

public class MessageReadStatusConfiguration : BaseEntityConfiguration<MessageReadStatus, Guid>
{
    public override void Configure(EntityTypeBuilder<MessageReadStatus> builder)
    {
        base.Configure(builder);

        builder.ToTable("MessageReadStatuses");

        builder.HasIndex(s => new { s.MessageID, s.UserID }).IsUnique();

        builder.HasOne(s => s.Message)
            .WithMany(m => m.ReadStatuses)
            .HasForeignKey(s => s.MessageID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
