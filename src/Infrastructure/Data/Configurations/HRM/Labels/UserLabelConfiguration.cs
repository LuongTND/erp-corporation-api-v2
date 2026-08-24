namespace Infrastructure;

public class UserLabelConfiguration : BaseEntityConfiguration<UserLabel, Guid>
{
    public override void Configure(EntityTypeBuilder<UserLabel> builder)
    {
        base.Configure(builder);
        builder.ToTable("UserLabels");
        builder.HasIndex(ul => new { ul.UserId, ul.LabelId }).IsUnique();
        builder.HasOne(ul => ul.User)
            .WithMany(u => u.UserLabels)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ul => ul.Label)
            .WithMany(l => l.UserLabels)
            .HasForeignKey(ul => ul.LabelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
