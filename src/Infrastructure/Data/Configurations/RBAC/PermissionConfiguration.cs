namespace Infrastructure;

public class PermissionConfiguration : BaseEntityConfiguration<Permission, Guid>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable("Permissions");

        builder.HasIndex(p => p.PermissionCode).IsUnique();
        builder.Property(p => p.PermissionCode).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Module).HasConversion<string>().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
    }
}
