namespace Application.DTOs.Users;

public record AddUserDepartmentRequest(
    Guid DepartmentId,
    DateOnly StartDate,
    DateOnly? EndDate = null
);
