namespace Infrastructure;

public class KpiEntryConfiguration : AuditableEntityConfiguration<KpiEntry, Guid>
{
    public override void Configure(EntityTypeBuilder<KpiEntry> builder)
    {
        base.Configure(builder);

        builder.ToTable("KpiEntries");

        builder.Property(k => k.ActualValue).HasPrecision(18, 4).IsRequired();
        builder.Property(k => k.Score).HasPrecision(5, 2).IsRequired();
        builder.Property(k => k.Note).HasMaxLength(500);

        builder.HasIndex(k => new { k.UserId, k.KpiMetricId, k.Month, k.Year }).IsUnique();

        builder.HasOne(k => k.User).WithMany().HasForeignKey(k => k.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(k => k.KpiMetric).WithMany().HasForeignKey(k => k.KpiMetricId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
