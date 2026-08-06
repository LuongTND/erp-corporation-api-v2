namespace Domain;

public class ConversationMember : EntityBase<Guid>
{
    public Guid ConversationID { get; set; }
    public Conversation? Conversation { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public RoleInConversation RoleInConversation { get; set; } = RoleInConversation.Member;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }

    public Guid? LastReadMessageID { get; set; }
    public Message? LastReadMessage { get; set; }

    public bool IsMuted { get; set; }
    public bool IsActive { get; set; }

    public void Leave()
    {
        IsActive = false;
        LeftAt = DateTimeOffset.UtcNow;
    }
}
