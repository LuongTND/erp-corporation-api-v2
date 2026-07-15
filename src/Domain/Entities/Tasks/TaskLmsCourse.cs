namespace Domain;
public class TaskLmsCourse
{
    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public Guid CourseID { get; private set; } // Liên kết logic với LMS Course

    public bool RequiredForCompletion { get; private set; }

    private TaskLmsCourse()
    {
    }

    public static TaskLmsCourse Create(Guid taskId, Guid courseId, bool requiredForCompletion = false)
    {
        return new TaskLmsCourse
        {
            TaskID = taskId,
            CourseID = courseId,
            RequiredForCompletion = requiredForCompletion
        };
    }
}
