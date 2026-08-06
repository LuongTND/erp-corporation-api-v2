namespace Application;

public sealed record GetUsersQuery(string? Search = null) : IRequest<IEnumerable<UserSummaryResponse>>;
