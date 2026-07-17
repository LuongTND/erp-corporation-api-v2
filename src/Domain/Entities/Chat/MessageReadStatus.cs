namespace Domain;

public class MessageReadStatus
{
    public Guid MessageID { get; private set; }
    public virtual Message Message { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private MessageReadStatus()
    {
    }

    public static MessageReadStatus Create(Guid messageId, Guid userId)
    {
        return new MessageReadStatus
        {
            MessageID = messageId,
            UserID = userId,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}