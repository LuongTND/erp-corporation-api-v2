namespace Infrastructure;

public class RegionConfiguration : AuditableEntityConfiguration<Region, Guid>
{
    public override void Configure(EntityTypeBuilder<Region> builder)
    {
        base.Configure(builder);
        builder.ToTable("Regions");
        builder.HasIndex(r => r.PosRegionId).IsUnique();
        builder.HasIndex(r => r.Code).IsUnique();
        builder.Property(r => r.Name).IsRequired().HasMaxLength(255);
        builder.Property(r => r.Code).IsRequired().HasMaxLength(50);
        builder.Property(r => r.PosRegionId).IsRequired().HasMaxLength(100);

        builder.HasOne(r => r.Manager)
            .WithMany()
            .HasForeignKey(r => r.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
