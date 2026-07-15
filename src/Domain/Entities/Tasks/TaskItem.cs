namespace Domain;
public class TaskItem : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public string TaskCode { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public TaskType TaskType { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public int Progress { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public decimal? EstimatedHours { get; private set; }
    public decimal? ActualHours { get; private set; }
    public bool IsRecurring { get; private set; }
    public RecurringPattern? RecurringPattern { get; private set; }
    public Guid? ParentTaskID { get; private set; }
    public virtual TaskItem? ParentTask { get; private set; }
    
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<TaskItem> Subtasks { get; private set; } = [];
    public virtual ICollection<TaskAssignee> Assignees { get; private set; } = [];
    public virtual ICollection<TaskFollower> Followers { get; private set; } = [];
    public virtual ICollection<TaskComment> Comments { get; private set; } = [];
    public virtual ICollection<TaskActivityLog> ActivityLogs { get; private set; } = [];
    public virtual ICollection<TaskKpi> TaskKpis { get; private set; } = [];
    public virtual ICollection<TaskLmsCourse> TaskLmsCourses { get; private set; } = [];

    private TaskItem() : base()
    {
    }

    public static TaskItem Create(
        string taskCode,
        string title,
        string? description,
        TaskType taskType,
        TaskPriority priority,
        DateTime? startDate = null,
        DateTime? dueDate = null,
        decimal? estimatedHours = null,
        bool isRecurring = false,
        RecurringPattern? recurringPattern = null,
        Guid? parentTaskId = null)
    {
        return new TaskItem
        {
            TaskCode = taskCode.Trim(),
            Title = title.Trim(),
            Description = description,
            TaskType = taskType,
            Status = TaskStatus.ToDo,
            Priority = priority,
            Progress = 0,
            StartDate = startDate,
            DueDate = dueDate,
            EstimatedHours = estimatedHours,
            IsRecurring = isRecurring,
            RecurringPattern = recurringPattern,
            ParentTaskID = parentTaskId,
            IsActive = true
        };
    }

    public void Update(
        string title,
        string? description,
        TaskType taskType,
        TaskPriority priority,
        DateTime? startDate = null,
        DateTime? dueDate = null,
        decimal? estimatedHours = null,
        bool isRecurring = false,
        RecurringPattern? recurringPattern = null,
        Guid? parentTaskId = null)
    {
        Title = title.Trim();
        Description = description;
        TaskType = taskType;
        Priority = priority;
        StartDate = startDate;
        DueDate = dueDate;
        EstimatedHours = estimatedHours;
        IsRecurring = isRecurring;
        RecurringPattern = recurringPattern;
        ParentTaskID = parentTaskId;
    }

    public void UpdateStatus(TaskStatus status)
    {
        Status = status;
        if (status == TaskStatus.Done)
        {
            Progress = 100;
            CompletedDate = DateTime.UtcNow;
        }
        else if (status == TaskStatus.ToDo)
        {
            Progress = 0;
            CompletedDate = null;
        }
    }

    public void UpdateProgress(int progress)
    {
        Progress = Math.Clamp(progress, 0, 100);
        if (Progress == 100)
        {
            Status = TaskStatus.Done;
            CompletedDate = DateTime.UtcNow;
        }
        else if (Progress > 0 && Status == TaskStatus.ToDo)
        {
            Status = TaskStatus.InProgress;
            CompletedDate = null;
        }
    }

    public void SetActualHours(decimal? actualHours)
    {
        ActualHours = actualHours;
    }
}
