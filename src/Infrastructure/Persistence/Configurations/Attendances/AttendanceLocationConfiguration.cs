using Domain.Entities.Attendances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Attendances;

public class AttendanceLocationConfiguration : IEntityTypeConfiguration<AttendanceLocation>
{
    public void Configure(EntityTypeBuilder<AttendanceLocation> builder)
    {
        builder.ToTable("AttendanceLocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.Property(x => x.RadiusInMeters)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
