namespace Infrastructure;

public class NotificationTemplateConfiguration : BaseEntityConfiguration<NotificationTemplate, Guid>
{
    public override void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationTemplates");

        builder.HasIndex(t => new { t.EventTypeId, t.Channel }).IsUnique();

        builder.Property(t => t.Channel).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.TitleTemplate).IsRequired().HasMaxLength(500);
        builder.Property(t => t.BodyTemplate).IsRequired().HasMaxLength(2000);

        builder.HasOne(t => t.EventType)
            .WithMany(e => e.Templates)
            .HasForeignKey(t => t.EventTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
