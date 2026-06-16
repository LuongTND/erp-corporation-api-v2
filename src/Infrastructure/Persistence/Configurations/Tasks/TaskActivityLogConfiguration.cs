using Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tasks;

public class TaskActivityLogConfiguration : IEntityTypeConfiguration<TaskActivityLog>
{
    public void Configure(EntityTypeBuilder<TaskActivityLog> builder)
    {
        builder.ToTable("Task_Activity_Log");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("LogID");

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.OldValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValue)
            .HasColumnType("nvarchar(max)");

        builder.HasOne(x => x.Task)
            .WithMany(t => t.ActivityLogs)
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
