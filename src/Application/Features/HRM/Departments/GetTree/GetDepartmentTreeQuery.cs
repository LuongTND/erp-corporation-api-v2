namespace Application;

public sealed record GetDepartmentTreeQuery : IRequest<IEnumerable<DepartmentTreeResponse>>;
