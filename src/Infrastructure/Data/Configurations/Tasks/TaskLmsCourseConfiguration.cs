namespace Infrastructure;

public class TaskLmsCourseConfiguration : BaseEntityConfiguration<TaskLmsCourse, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskLmsCourse> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskLmsCourses");

        builder.HasIndex(l => new { l.TaskID, l.CourseId }).IsUnique();

        builder.Property(l => l.CompletionStatus).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(l => l.Task)
            .WithMany(t => t.TaskLmsCourses)
            .HasForeignKey(l => l.TaskID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
