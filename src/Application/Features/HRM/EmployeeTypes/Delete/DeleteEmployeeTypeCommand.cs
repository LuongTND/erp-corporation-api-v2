namespace Application;

public sealed record DeleteEmployeeTypeCommand(Guid EmployeeTypeId) : IRequest<Unit>;
