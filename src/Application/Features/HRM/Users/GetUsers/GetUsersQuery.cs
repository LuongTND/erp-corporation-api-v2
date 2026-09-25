namespace Application;

public sealed record GetUsersQuery(string? Search = null, Guid? JobTitleId = null, UserStatus? Status = null, Guid? DepartmentId = null, Guid? LabelId = null, Guid? StoreId = null, Guid? RegionId = null, Guid CallerId = default) : IRequest<IEnumerable<UserSummaryResponse>>;
