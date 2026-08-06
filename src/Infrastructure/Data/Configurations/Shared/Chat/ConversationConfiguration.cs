namespace Infrastructure;

public class ConversationConfiguration : AuditableEntityConfiguration<Conversation, Guid>
{
    public override void Configure(EntityTypeBuilder<Conversation> builder)
    {
        base.Configure(builder);

        builder.ToTable("Conversations");

        builder.Property(c => c.ConversationType).HasConversion<string>().HasMaxLength(30);
        builder.Property(c => c.Title).HasMaxLength(255);
        builder.Property(c => c.Description).HasMaxLength(1000);
    }
}
