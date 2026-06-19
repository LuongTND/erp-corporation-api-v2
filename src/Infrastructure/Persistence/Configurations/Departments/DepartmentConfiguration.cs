using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Departments;

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

        builder.Property(x => x.CheckInTimeTarget)
            .HasMaxLength(5)
            .HasDefaultValue("08:00")
            .IsRequired();

        builder.Property(x => x.CheckOutTimeTarget)
            .HasMaxLength(5)
            .HasDefaultValue("17:00")
            .IsRequired();
    }
}
