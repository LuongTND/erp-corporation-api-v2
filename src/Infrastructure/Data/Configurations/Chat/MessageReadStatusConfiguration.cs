namespace Infrastructure;

public class MessageReadStatusConfiguration : IEntityTypeConfiguration<MessageReadStatus>
{
    public void Configure(EntityTypeBuilder<MessageReadStatus> builder)
    {
        builder.ToTable("Message_Read_Status");

        builder.HasKey(x => new { x.MessageID, x.UserID });

        builder.HasOne(x => x.Message)
            .WithMany(m => m.ReadStatuses)
            .HasForeignKey(x => x.MessageID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsRead)
            .IsRequired();
    }
}