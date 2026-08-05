namespace Application;

public sealed record UpdateUserDepartmentCommand(
    Guid UserId,
    Guid DepartmentId,
    Guid? JobLevelId
) : IRequest<Unit>;
