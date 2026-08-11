namespace Infrastructure;

public class KpiMetricConfiguration : BaseEntityConfiguration<KpiMetric, Guid>
{
    public override void Configure(EntityTypeBuilder<KpiMetric> builder)
    {
        base.Configure(builder);

        builder.ToTable("KpiMetrics");

        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Unit).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Weight).HasPrecision(5, 2);
        builder.Property(m => m.Target).HasPrecision(18, 4);
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(m => m.Template)
            .WithMany(t => t.Metrics)
            .HasForeignKey(m => m.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
