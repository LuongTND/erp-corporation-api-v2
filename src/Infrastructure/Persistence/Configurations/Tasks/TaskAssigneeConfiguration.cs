using Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tasks;

public class TaskAssigneeConfiguration : IEntityTypeConfiguration<TaskAssignee>
{
    public void Configure(EntityTypeBuilder<TaskAssignee> builder)
    {
        builder.ToTable("Task_Assignees");

        builder.HasKey(x => new { x.TaskID, x.UserID });

        builder.HasOne(x => x.Task)
            .WithMany(t => t.Assignees)
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.AssignedAt)
            .IsRequired();

        builder.Property(x => x.IsPrimaryAssignee)
            .IsRequired();
    }
}
