namespace Infrastructure;

public class EmployeeIdentityConfiguration : BaseEntityConfiguration<EmployeeIdentity, Guid>
{
    public override void Configure(EntityTypeBuilder<EmployeeIdentity> builder)
    {
        base.Configure(builder);

        builder.ToTable("EmployeeIdentities");

        builder.Property(e => e.IdentityCardNumber).HasMaxLength(20);
        builder.Property(e => e.IdentityCardIssuedPlace).HasMaxLength(255);
        builder.Property(e => e.PassportNumber).HasMaxLength(20);

        builder.HasIndex(e => e.IdentityCardNumber)
            .IsUnique()
            .HasFilter("[IdentityCardNumber] IS NOT NULL");
    }
}
