namespace Domain;

public class TaskItem : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string TaskCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; }

    public Guid StatusId { get; set; }
    public TaskItemStatus? Status { get; set; }

    public Guid PriorityId { get; set; }
    public TaskPriority? Priority { get; set; }

    public int Progress { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public RecurringPattern? RecurringPattern { get; set; }
    public Guid? ParentTaskId { get; set; }
    public TaskItem? ParentTask { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<TaskItem> Subtasks { get; set; } = [];
    public ICollection<TaskAssignee> Assignees { get; set; } = [];
    public ICollection<TaskFollower> Followers { get; set; } = [];
    public ICollection<TaskComment> Comments { get; set; } = [];
    public ICollection<TaskActivityLog> ActivityLogs { get; set; } = [];
    public ICollection<TaskKpi> TaskKpis { get; set; } = [];
    public ICollection<TaskLmsCourse> TaskLmsCourses { get; set; } = [];
    public ICollection<TaskAttachment> Attachments { get; set; } = [];
    public ICollection<TaskDependency> BlockingTasks { get; set; } = [];
    public ICollection<TaskDependency> BlockedByTasks { get; set; } = [];

    public void UpdateStatus(TaskItemStatus status)
    {
        StatusId = status.Id;
        Status = status;
        if (status.IsFinalState)
        {
            Progress = 100;
            CompletedDate = DateTimeOffset.UtcNow;
        }
        else if (status.IsInitialState)
        {
            Progress = 0;
            CompletedDate = null;
        }
    }

    public void UpdateProgress(int progress)
    {
        Progress = Math.Clamp(progress, 0, 100);
        if (Progress == 100 && Status?.IsFinalState == false)
        {
            CompletedDate = DateTimeOffset.UtcNow;
        }
        else if (Progress > 0 && Status?.IsInitialState == true)
        {
            CompletedDate = null;
        }
    }

    public void SetActualHours(decimal? actualHours) => ActualHours = actualHours;
}
