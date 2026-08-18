namespace Infrastructure;

public class EmploymentContractConfiguration : AuditableEntityConfiguration<EmploymentContract, Guid>
{
    public override void Configure(EntityTypeBuilder<EmploymentContract> builder)
    {
        base.Configure(builder);

        builder.ToTable("EmploymentContracts");

        builder.Property(c => c.ContractNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.ContractNumber).IsUnique();

        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(c => c.StartDate).IsRequired();
        builder.Property(c => c.Salary).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.SalaryForSocialInsurance).HasPrecision(18, 2);
        builder.Property(c => c.PositionTitle).HasMaxLength(200);
        builder.Property(c => c.FileUrl).HasMaxLength(1000);
        builder.Property(c => c.TerminationReason).HasMaxLength(500);

        // 1 nhân sự chỉ có 1 HĐ Active tại 1 thời điểm — enforce ở application layer
        builder.HasIndex(c => new { c.UserId, c.Status });

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.RenewedFromContract)
            .WithMany()
            .HasForeignKey(c => c.RenewedFromContractId)
            .OnDelete(DeleteBehavior.NoAction);

        // TemplateId FK configured in ContractTemplateConfiguration (HasMany side)
    }
}
