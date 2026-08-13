namespace Infrastructure;

public class StoreConfiguration : AuditableEntityConfiguration<Store, Guid>
{
    public override void Configure(EntityTypeBuilder<Store> builder)
    {
        base.Configure(builder);
        builder.ToTable("Stores");
        builder.HasIndex(s => s.PosStoreId).IsUnique();
        builder.HasIndex(s => s.Code).IsUnique();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(255);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.Property(s => s.PosStoreId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.HasOne(s => s.Region)
            .WithMany(r => r.Stores)
            .HasForeignKey(s => s.RegionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
