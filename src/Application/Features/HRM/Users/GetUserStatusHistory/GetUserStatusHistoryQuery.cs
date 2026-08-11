namespace Application;

public sealed record GetUserStatusHistoryQuery(Guid UserId) : IRequest<IEnumerable<UserStatusHistoryResponse>>;
