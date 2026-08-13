namespace Infrastructure;

public class StoreHoursConfiguration : IEntityTypeConfiguration<StoreHours>
{
    public void Configure(EntityTypeBuilder<StoreHours> builder)
    {
        builder.ToTable("StoreHours");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.DayOfWeek).HasConversion<int>();
        builder.HasOne(h => h.Store)
            .WithMany(s => s.StoreHours)
            .HasForeignKey(h => h.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(h => new { h.StoreId, h.DayOfWeek }).IsUnique();
    }
}
