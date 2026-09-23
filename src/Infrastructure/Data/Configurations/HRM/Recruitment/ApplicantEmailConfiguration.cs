namespace Infrastructure;

public sealed class ApplicantEmailConfiguration : IEntityTypeConfiguration<ApplicantEmail>
{
    public void Configure(EntityTypeBuilder<ApplicantEmail> b)
    {
        b.ToTable("ApplicantEmails");
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
    }
}
