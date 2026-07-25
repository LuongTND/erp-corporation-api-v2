namespace Infrastructure;

public class UserConfiguration : AuditableEntityConfiguration<User, Guid>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.EmployeeCode).IsUnique();

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.EmployeeCode).IsRequired().HasMaxLength(50);
        builder.Property(u => u.AvatarUrl).HasMaxLength(1000);
        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(u => u.JobLevel)
            .WithMany(j => j.Users)
            .HasForeignKey(u => u.JobLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Manager)
            .WithMany(u => u.DirectReports)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(u => u.UserAccount)
            .WithOne(a => a.User)
            .HasForeignKey<UserAccount>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
