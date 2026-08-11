namespace Infrastructure;

public class PayrollRunConfiguration : AuditableEntityConfiguration<PayrollRun, Guid>
{
    public override void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        base.Configure(builder);

        builder.ToTable("PayrollRuns");

        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Note).HasMaxLength(1000);

        builder.HasIndex(p => new { p.Month, p.Year });
    }
}
