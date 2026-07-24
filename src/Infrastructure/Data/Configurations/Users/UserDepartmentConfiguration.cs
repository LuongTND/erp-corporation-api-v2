namespace Infrastructure;

public class UserDepartmentConfiguration : BaseEntityConfiguration<UserDepartment, Guid>
{
    public override void Configure(EntityTypeBuilder<UserDepartment> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserDepartments");

        builder.HasIndex(ud => new { ud.UserId, ud.DepartmentId }).IsUnique();

        builder.HasOne(ud => ud.User)
            .WithMany(u => u.UserDepartments)
            .HasForeignKey(ud => ud.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ud => ud.Department)
            .WithMany(d => d.UserDepartments)
            .HasForeignKey(ud => ud.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
