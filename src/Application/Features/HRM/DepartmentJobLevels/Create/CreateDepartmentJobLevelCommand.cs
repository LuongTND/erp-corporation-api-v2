namespace Application;

public sealed record CreateDepartmentJobLevelCommand(
    Guid DepartmentId,
    Guid JobLevelId,
    Guid? BonusPolicyId
) : IRequest<Guid>;
