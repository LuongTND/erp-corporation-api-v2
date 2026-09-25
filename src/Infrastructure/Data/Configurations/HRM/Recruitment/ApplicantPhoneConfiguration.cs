namespace Infrastructure;

public sealed class ApplicantPhoneConfiguration : IEntityTypeConfiguration<ApplicantPhone>
{
    public void Configure(EntityTypeBuilder<ApplicantPhone> b)
    {
        b.ToTable("ApplicantPhones");
        b.HasKey(x => x.Id);
        b.Property(x => x.Phone).HasMaxLength(20).IsRequired();
    }
}
