namespace Application;

public sealed record AssignKpiTemplateCommand(
    Guid DepartmentJobLevelId,
    Guid? KpiTemplateId  // null = unassign
) : IRequest<Unit>;
