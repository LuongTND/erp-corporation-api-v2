namespace Application;

public sealed record GetMyDepartmentsQuery : IRequest<IEnumerable<DepartmentResponse>>;
