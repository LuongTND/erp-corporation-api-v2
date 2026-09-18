namespace Application;

public sealed record AddUserDepartmentCommand(
    Guid UserId,
    Guid DepartmentId,
    DateOnly StartDate,
    Guid? JobTitleId = null
) : IRequest<Guid>;
