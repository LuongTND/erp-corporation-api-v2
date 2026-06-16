using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Roles;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PermissionCode)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.PermissionCode)
            .IsUnique();

        builder.Property(x => x.PermissionName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Module)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Action)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Resource)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
