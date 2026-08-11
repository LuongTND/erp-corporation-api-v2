namespace Application;

public sealed record DeleteDepartmentJobLevelCommand(Guid Id) : IRequest<Unit>;
