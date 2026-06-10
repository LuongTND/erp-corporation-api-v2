using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Notifications;

public class NotificationEventTypeConfiguration : IEntityTypeConfiguration<NotificationEventType>
{
    public void Configure(EntityTypeBuilder<NotificationEventType> builder)
    {
        builder.ToTable("NotificationEventTypes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventCode).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.EventCode).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Module).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.DefaultTitleTemplate).HasMaxLength(500).IsRequired();
        builder.Property(x => x.DefaultBodyTemplate).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
    }
}

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Channel).HasConversion<int>().IsRequired();
        builder.Property(x => x.TitleTemplate).HasMaxLength(500).IsRequired();
        builder.Property(x => x.BodyTemplate).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => new { x.EventTypeId, x.Channel }).IsUnique();

        builder.HasOne(x => x.EventType)
            .WithMany(x => x.Templates)
            .HasForeignKey(x => x.EventTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotificationTriggerBindingConfiguration : IEntityTypeConfiguration<NotificationTriggerBinding>
{
    public void Configure(EntityTypeBuilder<NotificationTriggerBinding> builder)
    {
        builder.ToTable("NotificationTriggerBindings");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TriggerKey).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.TriggerKey).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Module).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.LinkUrlTemplate).HasMaxLength(500);
        builder.Property(x => x.RecipientRulesJson).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne(x => x.EventType)
            .WithMany(x => x.TriggerBindings)
            .HasForeignKey(x => x.EventTypeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("UserNotifications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TriggerKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.LinkUrl).HasMaxLength(500);
        builder.Property(x => x.IsRead).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.EventType)
            .WithMany()
            .HasForeignKey(x => x.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
