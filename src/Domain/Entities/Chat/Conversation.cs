namespace Domain;

public class Conversation : AuditableEntityBase<Guid>, ISoftDeletable
{
    public ConversationType ConversationType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset? LastMessageAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<ConversationMember> Members { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<ConversationActivityLog> ActivityLogs { get; set; } = [];
}
