namespace Domain;

public class MessageReadStatus : EntityBase<Guid>
{
    public Guid MessageID { get; set; }
    public Message? Message { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTimeOffset.UtcNow;
    }
}
