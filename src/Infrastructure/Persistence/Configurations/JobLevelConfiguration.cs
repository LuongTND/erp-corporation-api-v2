using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobLevelConfiguration : IEntityTypeConfiguration<JobLevel>
{
    public void Configure(EntityTypeBuilder<JobLevel> builder)
    {
        builder.ToTable("JobLevels");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LevelName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LevelOrder)
            .IsRequired();

        builder.Property(x => x.DefaultScopeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.BaseSalaryMin)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.BaseSalaryMax)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
