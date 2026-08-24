namespace Application;

public sealed record GetUserDetailQuery(Guid UserId, Guid CallerId = default) : IRequest<UserDetailResponse>;
