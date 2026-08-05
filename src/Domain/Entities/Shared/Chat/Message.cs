namespace Domain;

public class Message : SoftDeletableEntityBase<Guid>
{
    public Guid ConversationID { get; set; }
    public Conversation? Conversation { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public string? Content { get; set; }
    public MessageType MessageType { get; set; }

    public Guid? ParentMessageID { get; set; }
    public Message? ParentMessage { get; set; }

    public bool IsEdited { get; set; }
    public DateTimeOffset? EditedAt { get; set; }

    public ICollection<Message> Replies { get; set; } = [];
    public ICollection<MessageAttachment> Attachments { get; set; } = [];
    public ICollection<MessageReaction> Reactions { get; set; } = [];
    public ICollection<MessageReadStatus> ReadStatuses { get; set; } = [];
    public ICollection<MessageTask> MessageTasks { get; set; } = [];

    public void Edit(string newContent)
    {
        Content = newContent.Trim();
        IsEdited = true;
        EditedAt = DateTimeOffset.UtcNow;
    }

    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
        Content = null;
    }
}
