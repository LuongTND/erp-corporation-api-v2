namespace Infrastructure;

public class EmployeeTypeConfiguration : AuditableEntityConfiguration<EmployeeType, Guid>
{
    public override void Configure(EntityTypeBuilder<EmployeeType> builder)
    {
        base.Configure(builder);

        builder.ToTable("EmployeeTypes");

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
