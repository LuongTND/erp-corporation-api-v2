using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserDepartmentConfiguration : IEntityTypeConfiguration<UserDepartment>
{
    public void Configure(EntityTypeBuilder<UserDepartment> builder)
    {
        builder.ToTable("UserDepartments");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserDepartments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany(d => d.UserDepartments)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsPrimary)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Index for performance
        builder.HasIndex(x => new { x.UserId, x.DepartmentId });
    }
}
