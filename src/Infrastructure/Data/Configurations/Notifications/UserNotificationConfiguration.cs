namespace Infrastructure;

public class UserNotificationConfiguration : BaseEntityConfiguration<UserNotification, Guid>
{
    public override void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserNotifications");

        builder.HasIndex(n => new { n.UserId, n.IsRead });

        builder.Property(n => n.TriggerKey).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(500);
        builder.Property(n => n.Body).IsRequired().HasMaxLength(2000);
        builder.Property(n => n.LinkUrl).HasMaxLength(1000);
        builder.Property(n => n.Channel).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.EventType)
            .WithMany()
            .HasForeignKey(n => n.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
