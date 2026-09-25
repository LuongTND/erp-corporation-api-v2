namespace Domain;

public class UserStore : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid StoreId { get; set; }
    public Store? Store { get; set; }

    public bool IsHomeStore { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
