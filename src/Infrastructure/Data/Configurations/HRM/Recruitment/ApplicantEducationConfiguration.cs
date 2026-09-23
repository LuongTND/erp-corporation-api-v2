namespace Infrastructure;

public sealed class ApplicantEducationConfiguration : IEntityTypeConfiguration<ApplicantEducation>
{
    public void Configure(EntityTypeBuilder<ApplicantEducation> b)
    {
        b.ToTable("ApplicantEducations");
        b.HasKey(x => x.Id);
        b.Property(x => x.SchoolName).HasMaxLength(300);
        b.Property(x => x.Major).HasMaxLength(200);
    }
}
