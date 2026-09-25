namespace Application;

public sealed record DeleteDepartmentCommand(Guid DepartmentId) : IRequest<Unit>;
