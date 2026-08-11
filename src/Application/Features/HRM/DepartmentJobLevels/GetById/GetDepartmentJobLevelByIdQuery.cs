namespace Application;

public sealed record GetDepartmentJobLevelByIdQuery(Guid Id) : IRequest<DepartmentJobLevelResponse>;
