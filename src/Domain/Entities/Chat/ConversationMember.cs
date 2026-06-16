using Domain.Entities.Users;
using Domain.Enums.Chat;

namespace Domain.Entities.Chat;

public class ConversationMember
{
    public Guid ConversationID { get; private set; }
    public virtual Conversation Conversation { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public RoleInConversation? RoleInConversation { get; private set; }
    public DateTime JoinedAt { get; private set; }
    
    public Guid? LastReadMessageID { get; private set; }
    public virtual Message? LastReadMessage { get; private set; }

    public bool IsMuted { get; private set; }
    public bool IsActive { get; private set; }

    private ConversationMember()
    {
    }

    public static ConversationMember Create(
        Guid conversationId, 
        Guid userId, 
        Domain.Enums.Chat.RoleInConversation? role = Domain.Enums.Chat.RoleInConversation.Member)
    {
        return new ConversationMember
        {
            ConversationID = conversationId,
            UserID = userId,
            RoleInConversation = role,
            JoinedAt = DateTime.UtcNow,
            IsMuted = false,
            IsActive = true
        };
    }

    public void UpdateRole(RoleInConversation role)
    {
        RoleInConversation = role;
    }

    public void SetMuted(bool isMuted)
    {
        IsMuted = isMuted;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
        JoinedAt = DateTime.UtcNow;
    }

    public void UpdateLastReadMessage(Guid messageId)
    {
        LastReadMessageID = messageId;
    }
}
