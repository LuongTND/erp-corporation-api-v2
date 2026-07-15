
namespace Application;
public record CreateUserRequest(
    string EmployeeCode,
    string FullName,
    string Email,
    Guid DepartmentId,
    Guid JobLevelId,
    DateOnly DateOfJoin,
    UserStatus Status,
    Guid? ManagerId = null,
    string? AvatarUrl = null,
    string? Password = null
);
