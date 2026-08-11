namespace Infrastructure;

public class BonusPolicyConfiguration : AuditableEntityConfiguration<BonusPolicy, Guid>
{
    public override void Configure(EntityTypeBuilder<BonusPolicy> builder)
    {
        base.Configure(builder);

        builder.ToTable("BonusPolicies");

        builder.Property(b => b.Name).IsRequired().HasMaxLength(255);
        builder.Property(b => b.Description).HasMaxLength(1000);
    }
}
