namespace Application;

public sealed record UpdateUserDepartmentCommand(
    Guid UserId,
    Guid DepartmentId,
    Guid? JobTitleId
) : IRequest<Unit>;
