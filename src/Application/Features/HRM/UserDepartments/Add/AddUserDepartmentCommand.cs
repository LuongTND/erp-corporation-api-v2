namespace Application;

public sealed record AddUserDepartmentCommand(
    Guid UserId,
    Guid DepartmentId,
    DateOnly StartDate,
    Guid? JobLevelId = null
) : IRequest<Guid>;
