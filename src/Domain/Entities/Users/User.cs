
namespace Domain;
public class User : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public string EmployeeCode { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? AvatarUrl { get; private set; }

    public Guid DepartmentId { get; private set; }
    public virtual Department Department { get; private set; } = null!;

    public Guid JobLevelId { get; private set; }
    public virtual JobLevel JobLevel { get; private set; } = null!;

    public Guid? ManagerId { get; private set; }
    public virtual User? Manager { get; private set; }
    public virtual ICollection<User> DirectReports { get; private set; } = [];

    public DateOnly DateOfJoin { get; private set; }
    public UserStatus Status { get; private set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual UserAccount? UserAccount { get; private set; }
    public virtual ICollection<UserDepartment> UserDepartments { get; private set; } = [];
    public virtual ICollection<UserRole> UserRoles { get; private set; } = [];

    private User() : base()
    {
    }

    public static User Create(
        string employeeCode,
        string fullName,
        string email,
        Guid departmentId,
        Guid jobLevelId,
        DateOnly dateOfJoin,
        UserStatus status,
        Guid? managerId = null,
        string? avatarUrl = null)
    {
        return new User
        {
            EmployeeCode = employeeCode.Trim(),
            FullName = fullName,
            Email = email.ToLowerInvariant(),
            DepartmentId = departmentId,
            JobLevelId = jobLevelId,
            DateOfJoin = dateOfJoin,
            Status = status,
            ManagerId = managerId,
            AvatarUrl = avatarUrl,
            IsActive = IsStatusActive(status)
        };
    }

    public void UpdateProfile(
        string fullName,
        string email,
        Guid departmentId,
        Guid jobLevelId,
        DateOnly dateOfJoin,
        UserStatus status,
        Guid? managerId = null,
        string? avatarUrl = null)
    {
        FullName = fullName;
        Email = email.ToLowerInvariant();
        DepartmentId = departmentId;
        JobLevelId = jobLevelId;
        DateOfJoin = dateOfJoin;
        Status = status;
        ManagerId = managerId;
        AvatarUrl = avatarUrl;
        IsActive = IsStatusActive(status);
    }

    public void SetStatus(UserStatus status)
    {
        Status = status;
        IsActive = IsStatusActive(status);
    }

    private static bool IsStatusActive(UserStatus status)
    {
        return status switch
        {
            UserStatus.Active => true,
            UserStatus.Probation => true,
            UserStatus.MaternityLeave => true,
            _ => false
        };
    }
}
