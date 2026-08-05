namespace Domain;

public class MessageReaction : EntityBase<Guid>
{
    public Guid MessageID { get; set; }
    public Message? Message { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public string ReactionType { get; set; } = string.Empty;
}
