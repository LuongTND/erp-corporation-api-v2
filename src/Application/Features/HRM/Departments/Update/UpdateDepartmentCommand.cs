namespace Application;

public sealed record UpdateDepartmentCommand(
    Guid DepartmentId,
    string DepartmentName,
    string DepartmentCode,
    Guid? ParentDepartmentId,
    Guid? ManagerId,
    string? Description,
    bool IsActive
) : IRequest<Unit>;
