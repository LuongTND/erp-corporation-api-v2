namespace Domain;

public class UserStatusHistory : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public UserStatus OldStatus { get; set; }
    public UserStatus NewStatus { get; set; }
    public string? Note { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
}
