namespace Domain;

public class UserDepartment : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }

    public bool IsPrimary { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}
