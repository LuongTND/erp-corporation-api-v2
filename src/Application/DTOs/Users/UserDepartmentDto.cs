namespace Application;
public class UserDepartmentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string DepartmentCode { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsActive { get; set; }
}
