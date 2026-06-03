using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DepartmentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DepartmentCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.DepartmentCode)
            .IsUnique();

        builder.HasOne(x => x.ParentDepartment)
            .WithMany(x => x.ChildDepartments)
            .HasForeignKey(x => x.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Manager)
            .WithMany()
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
