using Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Chat;

public class MessageTaskConfiguration : IEntityTypeConfiguration<MessageTask>
{
    public void Configure(EntityTypeBuilder<MessageTask> builder)
    {
        builder.ToTable("Message_Tasks");

        builder.HasKey(x => new { x.MessageID, x.TaskID });

        builder.HasOne(x => x.Message)
            .WithMany(m => m.MessageTasks)
            .HasForeignKey(x => x.MessageID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Task)
            .WithMany()
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.LinkedAt)
            .IsRequired();
    }
}
