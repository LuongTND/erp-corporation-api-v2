using Domain.Entities.Attendances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Attendances;

public class AttendanceLocationDepartmentConfiguration : IEntityTypeConfiguration<AttendanceLocationDepartment>
{
    public void Configure(EntityTypeBuilder<AttendanceLocationDepartment> builder)
    {
        builder.ToTable("AttendanceLocationDepartments");

        builder.HasKey(x => new { x.AttendanceLocationId, x.DepartmentId });

        builder.HasOne(x => x.AttendanceLocation)
            .WithMany(al => al.AssignedDepartments)
            .HasForeignKey(x => x.AttendanceLocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
