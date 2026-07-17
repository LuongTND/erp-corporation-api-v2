
namespace Infrastructure;
public class TaskLmsCourseConfiguration : IEntityTypeConfiguration<TaskLmsCourse>
{
    public void Configure(EntityTypeBuilder<TaskLmsCourse> builder)
    {
        builder.ToTable("Task_LMS_Courses");

        builder.HasKey(x => new { x.TaskID, x.CourseID });

        builder.HasOne(x => x.Task)
            .WithMany(t => t.TaskLmsCourses)
            .HasForeignKey(x => x.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.RequiredForCompletion)
            .IsRequired();
    }
}
