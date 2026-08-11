namespace Application;

public sealed record GetUsersQuery(string? Search = null, Guid? JobLevelId = null, UserStatus? Status = null, Guid? DepartmentId = null) : IRequest<IEnumerable<UserSummaryResponse>>;
