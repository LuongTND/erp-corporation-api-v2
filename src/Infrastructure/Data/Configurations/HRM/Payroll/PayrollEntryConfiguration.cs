namespace Infrastructure;

public class PayrollEntryConfiguration : AuditableEntityConfiguration<PayrollEntry, Guid>
{
    public override void Configure(EntityTypeBuilder<PayrollEntry> builder)
    {
        base.Configure(builder);

        builder.ToTable("PayrollEntries");

        builder.Property(p => p.HourlyRateSnapshot).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.HoursWorked).HasPrecision(8, 2).IsRequired();
        builder.Property(p => p.GrossPay).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.BonusAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.TotalDeductions).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.NetPay).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.SocialInsurance).HasPrecision(18, 2);
        builder.Property(p => p.HealthInsurance).HasPrecision(18, 2);
        builder.Property(p => p.UnemploymentIns).HasPrecision(18, 2);
        builder.Property(p => p.PersonalIncomeTax).HasPrecision(18, 2);
        builder.Property(p => p.Note).HasMaxLength(500);

        builder.HasIndex(p => new { p.PayrollRunId, p.UserId }).IsUnique();

        builder.HasOne(p => p.PayrollRun).WithMany(r => r.Entries)
            .HasForeignKey(p => p.PayrollRunId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
