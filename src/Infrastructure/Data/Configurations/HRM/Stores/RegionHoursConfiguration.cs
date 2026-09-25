namespace Infrastructure;

public class RegionHoursConfiguration : IEntityTypeConfiguration<RegionHours>
{
    public void Configure(EntityTypeBuilder<RegionHours> builder)
    {
        builder.ToTable("RegionHours");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.DayOfWeek).HasConversion<int>();
        builder.HasOne(h => h.Region)
            .WithMany(r => r.RegionHours)
            .HasForeignKey(h => h.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(h => new { h.RegionId, h.DayOfWeek }).IsUnique();
    }
}
