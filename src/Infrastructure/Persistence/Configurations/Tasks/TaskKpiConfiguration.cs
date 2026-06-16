using Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tasks;

public class TaskKpiConfiguration : IEntityTypeConfiguration<TaskKpi>
{
    public void Configure(EntityTypeBuilder<TaskKpi> builder)
    {
        builder.ToTable("Task_KPIs");

        builder.HasKey(x => new { x.TaskID, x.KPIID });

        builder.HasOne(x => x.Task)
            .WithMany(t => t.TaskKpis)
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Weight)
            .HasPrecision(5, 2);
    }
}
