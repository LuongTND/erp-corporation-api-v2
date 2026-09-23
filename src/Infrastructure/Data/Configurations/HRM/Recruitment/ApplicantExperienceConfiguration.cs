namespace Infrastructure;

public sealed class ApplicantExperienceConfiguration : IEntityTypeConfiguration<ApplicantExperience>
{
    public void Configure(EntityTypeBuilder<ApplicantExperience> b)
    {
        b.ToTable("ApplicantExperiences");
        b.HasKey(x => x.Id);
        b.Property(x => x.RecentWorkplace).HasMaxLength(200);
        b.Property(x => x.CompanyName).HasMaxLength(300).IsRequired();
        b.Property(x => x.Position).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(2000);
    }
}
