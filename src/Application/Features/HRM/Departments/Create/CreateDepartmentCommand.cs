namespace Application;

public sealed record CreateDepartmentCommand(
    string DepartmentName,
    string DepartmentCode,
    Guid? ParentDepartmentId,
    Guid? ManagerId,
    string? Description
) : IRequest<Guid>;
