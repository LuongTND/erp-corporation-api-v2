namespace Application.DTOs.Departments;

public record UpdateDepartmentRequest(
    string DepartmentName,
    string DepartmentCode,
    Guid? ParentDepartmentId = null,
    Guid? ManagerId = null,
    string? Description = null,
    bool IsActive = true,
    string CheckInTimeTarget = "08:00",
    string CheckOutTimeTarget = "17:00"
);
