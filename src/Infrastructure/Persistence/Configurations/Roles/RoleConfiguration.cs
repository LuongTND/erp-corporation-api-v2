namespace Infrastructure;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoleName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.RoleName)
            .IsUnique();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IsSystemRole)
            .IsRequired();

        builder.Property(x => x.BypassDataScope)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}