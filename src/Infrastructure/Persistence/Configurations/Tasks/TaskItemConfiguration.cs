using Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tasks;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("TaskID");

        builder.Property(x => x.TaskCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.TaskCode)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TaskType)
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Progress)
            .IsRequired();

        builder.Property(x => x.RecurringPattern)
            .HasMaxLength(100)
            .HasConversion<string>();

        builder.HasOne(x => x.ParentTask)
            .WithMany(t => t.Subtasks)
            .HasForeignKey(x => x.ParentTaskID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
