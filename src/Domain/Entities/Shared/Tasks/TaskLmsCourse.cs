namespace Domain;

public class TaskLmsCourse : EntityBase<Guid>
{
    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    // Cross-module reference — LMS module quản lý entity này
    public Guid CourseId { get; set; }

    public bool RequiredForCompletion { get; set; }
    public CourseCompletionStatus CompletionStatus { get; set; } = CourseCompletionStatus.NotStarted;
    public DateTimeOffset? CompletedAt { get; set; }

    public void MarkCompleted()
    {
        CompletionStatus = CourseCompletionStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }
}
