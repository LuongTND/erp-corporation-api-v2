namespace Application;
public record AddUserDepartmentRequest(
    Guid DepartmentId,
    DateOnly StartDate,
    DateOnly? EndDate = null
);
