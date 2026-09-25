namespace Infrastructure;

public class DepartmentConfiguration : AuditableEntityConfiguration<Department, Guid>
{
    public override void Configure(EntityTypeBuilder<Department> builder)
    {
        base.Configure(builder);

        builder.ToTable("Departments");

        builder.HasIndex(d => d.DepartmentCode).IsUnique();

        builder.Property(d => d.DepartmentName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.DepartmentCode).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Description).HasMaxLength(1000);

        builder.HasOne(d => d.ParentDepartment)
            .WithMany(d => d.ChildDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
