namespace Infrastructure;

public class EmploymentInfoConfiguration : BaseEntityConfiguration<EmploymentInfo, Guid>
{
    public override void Configure(EntityTypeBuilder<EmploymentInfo> builder)
    {
        base.Configure(builder);

        builder.ToTable("EmploymentInfos");

        builder.Property(e => e.ContractType).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.TaxCode).HasMaxLength(20);
        builder.Property(e => e.SocialInsuranceCode).HasMaxLength(20);
        builder.Property(e => e.BankName).HasMaxLength(100);
        builder.Property(e => e.BankAccountNumber).HasMaxLength(30);
        builder.Property(e => e.BankBranch).HasMaxLength(200);
    }
}
