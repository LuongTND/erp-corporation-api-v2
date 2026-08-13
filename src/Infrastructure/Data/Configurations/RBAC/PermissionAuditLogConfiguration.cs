namespace Infrastructure;

public class PermissionAuditLogConfiguration : IEntityTypeConfiguration<PermissionAuditLog>
{
    public void Configure(EntityTypeBuilder<PermissionAuditLog> builder)
    {
        builder.ToTable("PermissionAuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Action).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ActorName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TargetUserName).HasMaxLength(200);
        builder.Property(x => x.RoleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PermissionCodes).HasMaxLength(4000);
        builder.Property(x => x.Detail).HasMaxLength(500);
        builder.HasIndex(x => x.OccurredAt);
        builder.HasIndex(x => x.ActorId);
        builder.HasIndex(x => x.RoleId);
    }
}
