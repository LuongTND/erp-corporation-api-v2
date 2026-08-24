namespace Application;

public sealed record GetUsersQuery(string? Search = null, Guid? JobLevelId = null, UserStatus? Status = null, Guid? DepartmentId = null, Guid? LabelId = null, Guid? StoreId = null, Guid? RegionId = null, Guid CallerId = default) : IRequest<IEnumerable<UserSummaryResponse>>;
