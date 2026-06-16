using Domain.Base;
using Domain.Entities.Users;
using Domain.Enums.Chat;

namespace Domain.Entities.Chat;

public class Message : BaseEntity
{
    public Guid ConversationID { get; private set; }
    public virtual Conversation Conversation { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public string? Content { get; private set; }
    public MessageType MessageType { get; private set; }

    public Guid? ParentMessageID { get; private set; }
    public virtual Message? ParentMessage { get; private set; }

    public bool IsEdited { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? EditedAt { get; private set; }

    public virtual ICollection<Message> Replies { get; private set; } = [];
    public virtual ICollection<MessageAttachment> Attachments { get; private set; } = [];
    public virtual ICollection<MessageReaction> Reactions { get; private set; } = [];
    public virtual ICollection<MessageReadStatus> ReadStatuses { get; private set; } = [];
    public virtual ICollection<MessageTask> MessageTasks { get; private set; } = [];

    private Message() : base()
    {
    }

    public static Message Create(
        Guid conversationId,
        Guid userId,
        string? content,
        MessageType messageType,
        Guid? parentMessageId = null)
    {
        return new Message
        {
            ConversationID = conversationId,
            UserID = userId,
            Content = content?.Trim(),
            MessageType = messageType,
            ParentMessageID = parentMessageId,
            IsEdited = false,
            IsDeleted = false
        };
    }

    public void Edit(string newContent)
    {
        Content = newContent.Trim();
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        Content = null; // Clear content for safety when message is deleted/recalled
    }
}
