
namespace Application;
public record UpdateUserRequest(
    string FullName,
    string Email,
    Guid DepartmentId,
    Guid JobLevelId,
    DateOnly DateOfJoin,
    UserStatus Status,
    Guid? ManagerId = null,
    string? AvatarUrl = null
);
// For updating password, locking account, or assigning roles, we can use dedicated endpoints/methods.
