
namespace Domain;
public class TaskComment : BaseEntity
{
    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public string Content { get; private set; } = null!;

    public Guid? ParentCommentID { get; private set; }
    public virtual TaskComment? ParentComment { get; private set; }

    public virtual ICollection<TaskComment> Replies { get; private set; } = [];

    private TaskComment() : base()
    {
    }

    public static TaskComment Create(Guid taskId, Guid userId, string content, Guid? parentCommentId = null)
    {
        return new TaskComment
        {
            TaskID = taskId,
            UserID = userId,
            Content = content.Trim(),
            ParentCommentID = parentCommentId
        };
    }
}
