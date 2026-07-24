namespace Domain;

public class MessageTask : EntityBase<Guid>
{
    public Guid MessageID { get; set; }
    public Message? Message { get; set; }

    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    public DateTimeOffset LinkedAt { get; set; }
}
