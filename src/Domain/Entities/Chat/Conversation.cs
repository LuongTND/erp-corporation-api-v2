
namespace Domain;
public class Conversation : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public ConversationType ConversationType { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsPrivate { get; private set; }
    public bool IsArchived { get; private set; }
    
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ConversationMember> Members { get; private set; } = [];
    public virtual ICollection<Message> Messages { get; private set; } = [];
    public virtual ICollection<ConversationActivityLog> ActivityLogs { get; private set; } = [];

    private Conversation() : base()
    {
    }

    public static Conversation Create(
        ConversationType conversationType,
        string? title,
        string? description,
        bool isPrivate,
        Guid createdBy)
    {
        return new Conversation
        {
            ConversationType = conversationType,
            Title = title?.Trim(),
            Description = description?.Trim(),
            IsPrivate = isPrivate,
            IsArchived = false,
            CreatedBy = createdBy,
            IsActive = true
        };
    }

    public void Update(string? title, string? description, bool isPrivate)
    {
        Title = title?.Trim();
        Description = description?.Trim();
        IsPrivate = isPrivate;
    }

    public void SetArchived(bool isArchived)
    {
        IsArchived = isArchived;
    }
}
