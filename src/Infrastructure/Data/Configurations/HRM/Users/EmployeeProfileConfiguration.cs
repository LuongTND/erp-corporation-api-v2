namespace Infrastructure;

public class EmployeeProfileConfiguration : BaseEntityConfiguration<EmployeeProfile, Guid>
{
    public override void Configure(EntityTypeBuilder<EmployeeProfile> builder)
    {
        base.Configure(builder);

        builder.ToTable("EmployeeProfiles");

        builder.Property(p => p.Gender).HasConversion<string>().HasMaxLength(10);
        builder.Property(p => p.PhoneNumber).HasMaxLength(20);
        builder.Property(p => p.PermanentAddress).HasMaxLength(500);
        builder.Property(p => p.CurrentAddress).HasMaxLength(500);
    }
}
