namespace Application;

public sealed record AssignEmployeeTypeCommand(
    Guid UserId,
    Guid? EmployeeTypeId
) : IRequest<Unit>;
