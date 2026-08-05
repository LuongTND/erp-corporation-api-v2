namespace Application;

public sealed record RemoveUserDepartmentCommand(Guid UserId, Guid DepartmentId) : IRequest<Unit>;
