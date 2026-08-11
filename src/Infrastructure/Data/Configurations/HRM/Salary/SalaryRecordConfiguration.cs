namespace Infrastructure;

public class SalaryRecordConfiguration : AuditableEntityConfiguration<SalaryRecord, Guid>
{
    public override void Configure(EntityTypeBuilder<SalaryRecord> builder)
    {
        base.Configure(builder);

        builder.ToTable("SalaryRecords");

        builder.Property(s => s.HourlyRate).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.Reason).HasMaxLength(500);

        builder.HasIndex(s => new { s.UserId, s.EffectiveTo });

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
