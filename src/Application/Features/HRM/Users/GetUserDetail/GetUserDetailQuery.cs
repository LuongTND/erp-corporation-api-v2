namespace Application;

public sealed record GetUserDetailQuery(Guid UserId) : IRequest<UserDetailResponse>;
