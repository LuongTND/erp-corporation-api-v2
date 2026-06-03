using Domain.Enums;

namespace Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string EmployeeCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public Guid JobLevelId { get; set; }
    public string JobLevelName { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public DateOnly DateOfJoin { get; set; }
    public UserStatus Status { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = [];
}
