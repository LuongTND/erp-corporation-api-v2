using Domain.Entities.Attendances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Attendances;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CheckInLocation)
            .WithMany()
            .HasForeignKey(x => x.CheckInLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CheckOutLocation)
            .WithMany()
            .HasForeignKey(x => x.CheckOutLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Đảm bảo mỗi nhân viên chỉ có tối đa 1 bản ghi chấm công tổng hợp mỗi ngày
        builder.HasIndex(x => new { x.UserId, x.Date })
            .IsUnique();
    }
}
