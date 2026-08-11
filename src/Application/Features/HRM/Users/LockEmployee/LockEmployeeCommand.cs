namespace Application;

public sealed record LockEmployeeCommand(Guid UserId, bool Lock) : IRequest<Unit>;
