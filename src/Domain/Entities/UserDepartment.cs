using Domain.Base;

namespace Domain.Entities;

public class UserDepartment : BaseEntity, IAuditable
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    public Guid DepartmentId { get; private set; }
    public virtual Department Department { get; private set; } = null!;

    public bool IsPrimary { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public bool IsActive { get; set; } = true;

    private UserDepartment() : base()
    {
    }

    public static UserDepartment Create(
        Guid userId,
        Guid departmentId,
        bool isPrimary,
        DateOnly startDate,
        DateOnly? endDate = null)
    {
        return new UserDepartment
        {
            UserId = userId,
            DepartmentId = departmentId,
            IsPrimary = isPrimary,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true
        };
    }

    public void Terminate(DateOnly endDate)
    {
        EndDate = endDate;
        IsActive = false;
    }
}
