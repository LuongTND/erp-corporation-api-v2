namespace Application;

public sealed record AddBulkUserDepartmentCommand(
    Guid DepartmentId,
    IEnumerable<Guid> UserIds,
    DateOnly StartDate
) : IRequest<int>;
