namespace Application.DTOs.Departments;

public record CreateDepartmentRequest(
    string DepartmentName,
    string DepartmentCode,
    Guid? ParentDepartmentId = null,
    Guid? ManagerId = null,
    string? Description = null,
    string CheckInTimeTarget = "08:00",
    string CheckOutTimeTarget = "17:00"
);
