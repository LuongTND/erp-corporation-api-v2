namespace Domain;

public class ConversationActivityLog : EntityBase<Guid>
{
    public Guid ConversationID { get; set; }
    public Conversation? Conversation { get; set; }

    public Guid? UserID { get; set; }
    public User? User { get; set; }

    public ConversationActivityAction Action { get; set; }
    public string? Description { get; set; }
}
