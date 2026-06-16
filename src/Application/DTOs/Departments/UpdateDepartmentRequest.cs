namespace Application.DTOs.Departments;

public record UpdateDepartmentRequest(
    string DepartmentName,
    string DepartmentCode,
    Guid? ParentDepartmentId = null,
    Guid? ManagerId = null,
    string? Description = null,
    bool IsActive = true
);
