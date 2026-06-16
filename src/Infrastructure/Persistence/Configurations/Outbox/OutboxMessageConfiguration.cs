using Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.Error).HasMaxLength(4000);

        builder.HasIndex(x => x.ProcessedAt);
        builder.HasIndex(x => new { x.ProcessedAt, x.CreatedAt });
    }
}
