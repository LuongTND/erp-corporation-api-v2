namespace Infrastructure;

public class NotificationTriggerBindingConfiguration : BaseEntityConfiguration<NotificationTriggerBinding, Guid>
{
    public override void Configure(EntityTypeBuilder<NotificationTriggerBinding> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationTriggerBindings");

        builder.HasIndex(b => b.TriggerKey).IsUnique();

        builder.Property(b => b.TriggerKey).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(255);
        builder.Property(b => b.Module).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Description).HasMaxLength(500);
        builder.Property(b => b.LinkUrlTemplate).HasMaxLength(1000);
        builder.Property(b => b.RecipientRulesJson).IsRequired().HasMaxLength(4000);

        builder.HasOne(b => b.EventType)
            .WithMany(e => e.TriggerBindings)
            .HasForeignKey(b => b.EventTypeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
