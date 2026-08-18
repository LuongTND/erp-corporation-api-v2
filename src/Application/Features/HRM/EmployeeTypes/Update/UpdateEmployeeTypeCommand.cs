namespace Application;

public sealed record UpdateEmployeeTypeCommand(
    Guid EmployeeTypeId,
    string Name,
    string Code,
    string? Description,
    bool IsActive
) : IRequest<Unit>;
