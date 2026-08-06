namespace Domain;

public class TaskComment : EntityBase<Guid>
{
    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid? ParentCommentID { get; set; }
    public TaskComment? ParentComment { get; set; }

    public ICollection<TaskComment> Replies { get; set; } = [];
}
