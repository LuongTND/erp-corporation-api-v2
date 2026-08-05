namespace Application;

public sealed record TransferUserDepartmentCommand(
    Guid UserId,
    Guid NewDepartmentId,
    DateOnly TransferDate
) : IRequest<Unit>;
