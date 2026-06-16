using Domain.Base;
using Domain.Entities.Users;
using Domain.Enums.Chat;

namespace Domain.Entities.Chat;

public class ConversationActivityLog : BaseEntity
{
    public Guid ConversationID { get; private set; }
    public virtual Conversation Conversation { get; private set; } = null!;

    public Guid? UserID { get; private set; }
    public virtual User? User { get; private set; }

    public ConversationActivityAction Action { get; private set; }
    public string? Description { get; private set; }

    private ConversationActivityLog() : base()
    {
    }

    public static ConversationActivityLog Create(
        Guid conversationId,
        Guid? userId,
        ConversationActivityAction action,
        string? description = null)
    {
        return new ConversationActivityLog
        {
            ConversationID = conversationId,
            UserID = userId,
            Action = action,
            Description = description?.Trim()
        };
    }
}
