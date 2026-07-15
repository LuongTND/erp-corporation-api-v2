namespace Application;

public record CreateDepartmentRequest(
    string DepartmentName,
    string DepartmentCode,
    Guid? ParentDepartmentId = null,
    Guid? ManagerId = null,
    string? Description = null
);