namespace Application;

public sealed record GetDepartmentByIdQuery(Guid DepartmentId) : IRequest<DepartmentResponse>;
