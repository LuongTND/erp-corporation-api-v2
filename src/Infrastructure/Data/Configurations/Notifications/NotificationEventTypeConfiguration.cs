namespace Infrastructure;

public class NotificationEventTypeConfiguration : BaseEntityConfiguration<NotificationEventType, Guid>
{
    public override void Configure(EntityTypeBuilder<NotificationEventType> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationEventTypes");

        builder.HasIndex(e => e.EventCode).IsUnique();

        builder.Property(e => e.EventCode).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Module).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.DefaultTitleTemplate).IsRequired().HasMaxLength(500);
        builder.Property(e => e.DefaultBodyTemplate).IsRequired().HasMaxLength(2000);
    }
}
