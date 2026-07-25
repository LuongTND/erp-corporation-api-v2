namespace Infrastructure;

public class UserAccountConfiguration : BaseEntityConfiguration<UserAccount, Guid>
{
    public override void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserAccounts");

        builder.HasIndex(a => a.LoginEmail).IsUnique();

        builder.Property(a => a.LoginEmail).IsRequired().HasMaxLength(255);
        builder.Property(a => a.PasswordHash).HasMaxLength(500);
        builder.Property(a => a.RefreshToken).HasMaxLength(500);
    }
}
