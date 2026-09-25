namespace Application;

public sealed record GetDepartmentMembersQuery(Guid DepartmentId) : IRequest<IEnumerable<DepartmentMemberResponse>>;
