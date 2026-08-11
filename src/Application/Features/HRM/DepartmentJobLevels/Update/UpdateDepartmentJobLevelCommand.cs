namespace Application;

public sealed record UpdateDepartmentJobLevelCommand(
    Guid Id,
    Guid? BonusPolicyId
) : IRequest<Unit>;
