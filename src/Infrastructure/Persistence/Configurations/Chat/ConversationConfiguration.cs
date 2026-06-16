using Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Chat;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ConversationID");

        builder.Property(x => x.ConversationType)
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(255);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsPrivate)
            .IsRequired();

        builder.Property(x => x.IsArchived)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
