namespace Infrastructure;

public class UserStoreConfiguration : BaseEntityConfiguration<UserStore, Guid>
{
    public override void Configure(EntityTypeBuilder<UserStore> builder)
    {
        base.Configure(builder);
        builder.ToTable("UserStores");
        // ponytail: no DB unique — app checks active duplicate before insert to allow rejoin after inactive
        builder.HasIndex(us => new { us.UserId, us.StoreId });
        builder.HasOne(us => us.User)
            .WithMany(u => u.UserStores)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(us => us.Store)
            .WithMany(s => s.UserStores)
            .HasForeignKey(us => us.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
