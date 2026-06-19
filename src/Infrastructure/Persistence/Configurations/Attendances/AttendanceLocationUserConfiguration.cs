using Domain.Entities.Attendances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Attendances;

public class AttendanceLocationUserConfiguration : IEntityTypeConfiguration<AttendanceLocationUser>
{
    public void Configure(EntityTypeBuilder<AttendanceLocationUser> builder)
    {
        builder.ToTable("AttendanceLocationUsers");

        builder.HasKey(x => new { x.AttendanceLocationId, x.UserId });

        builder.HasOne(x => x.AttendanceLocation)
            .WithMany(al => al.AssignedUsers)
            .HasForeignKey(x => x.AttendanceLocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
