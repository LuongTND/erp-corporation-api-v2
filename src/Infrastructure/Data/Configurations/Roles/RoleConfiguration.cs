using Role = Domain.Role;

namespace Infrastructure;

public class RoleConfiguration : BaseEntityConfiguration<Role, Guid>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.ToTable("Roles");

        builder.HasIndex(r => r.RoleName).IsUnique();

        builder.Property(r => r.RoleName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Description).HasMaxLength(500);
    }
}
