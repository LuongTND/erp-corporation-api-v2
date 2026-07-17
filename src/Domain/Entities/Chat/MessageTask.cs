namespace Domain;

public class MessageTask
{
    public Guid MessageID { get; private set; }
    public virtual Message Message { get; private set; } = null!;

    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public DateTime LinkedAt { get; private set; }

    private MessageTask()
    {
    }

    public static MessageTask Create(Guid messageId, Guid taskId)
    {
        return new MessageTask
        {
            MessageID = messageId,
            TaskID = taskId,
            LinkedAt = DateTime.UtcNow
        };
    }
}