using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("UserAccounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LoginEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.LoginEmail)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithOne(u => u.UserAccount)
            .HasForeignKey<UserAccount>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsLocked)
            .IsRequired();

        builder.Property(x => x.FailedLoginCount)
            .IsRequired();
    }
}
