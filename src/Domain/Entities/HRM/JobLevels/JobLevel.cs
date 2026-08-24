namespace Domain;

public class JobLevel : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string LevelName { get; set; } = string.Empty;
    public int LevelOrder { get; set; }
    public string? Description { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<User> Users { get; set; } = [];
}
