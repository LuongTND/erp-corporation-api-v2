namespace Domain;

public class MessageReaction : BaseEntity
{
    public Guid MessageID { get; private set; }
    public virtual Message Message { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public string ReactionType { get; private set; } = null!;

    private MessageReaction() : base()
    {
    }

    public static MessageReaction Create(Guid messageId, Guid userId, string reactionType)
    {
        return new MessageReaction
        {
            MessageID = messageId,
            UserID = userId,
            ReactionType = reactionType.Trim()
        };
    }
}