using Domain.Enums.Tasks;
using TaskStatus = Domain.Enums.Tasks.TaskStatus;

namespace Application.DTOs.Tasks;

public class TaskDto
{
    public Guid Id { get; set; }
    public string TaskCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int Progress { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public bool IsRecurring { get; set; }
    public RecurringPattern? RecurringPattern { get; set; }
    public Guid? ParentTaskID { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<TaskAssigneeDto> Assignees { get; set; } = [];
    public List<TaskFollowerDto> Followers { get; set; } = [];
    public List<TaskCommentDto> Comments { get; set; } = [];
    public List<TaskActivityLogDto> ActivityLogs { get; set; } = [];
    public List<Guid> KpiIds { get; set; } = [];
    public List<Guid> LmsCourseIds { get; set; } = [];
    public List<TaskDto> Subtasks { get; set; } = [];
}

public class TaskAssigneeDto
{
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string EmployeeCode { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }
    public bool IsPrimaryAssignee { get; set; }
}

public class TaskFollowerDto
{
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string EmployeeCode { get; set; } = null!;
    public DateTime FollowedAt { get; set; }
}

public class TaskCommentDto
{
    public Guid Id { get; set; }
    public Guid TaskID { get; set; }
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Content { get; set; } = null!;
    public Guid? ParentCommentID { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskCommentDto> Replies { get; set; } = [];
}

public class TaskActivityLogDto
{
    public Guid Id { get; set; }
    public Guid TaskID { get; set; }
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public TaskActivityAction Action { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; } = TaskType.Normal;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public bool IsRecurring { get; set; } = false;
    public RecurringPattern? RecurringPattern { get; set; }
    public Guid? ParentTaskID { get; set; }
    public List<Guid> AssigneeIds { get; set; } = [];
    public Guid? PrimaryAssigneeId { get; set; }
    public List<Guid> FollowerIds { get; set; } = [];
    public List<Guid> KpiIds { get; set; } = [];
    public List<Guid> LmsCourseIds { get; set; } = [];
}

public class UpdateTaskRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int Progress { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public bool IsRecurring { get; set; }
    public RecurringPattern? RecurringPattern { get; set; }
    public Guid? ParentTaskID { get; set; }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = null!;
    public Guid? ParentCommentID { get; set; }
}
