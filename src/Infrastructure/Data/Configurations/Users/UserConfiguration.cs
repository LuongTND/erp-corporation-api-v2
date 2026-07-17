namespace Infrastructure;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.EmployeeCode)
            .IsUnique();

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.AvatarUrl)
            .HasMaxLength(500);

        builder.HasOne(x => x.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JobLevel)
            .WithMany(j => j.Users)
            .HasForeignKey(x => x.JobLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Manager)
            .WithMany(m => m.DirectReports)
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DateOfJoin)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}