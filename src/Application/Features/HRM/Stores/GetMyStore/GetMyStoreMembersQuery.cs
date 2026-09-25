namespace Application;

public sealed record GetMyStoreMembersQuery : IRequest<IEnumerable<StoreMemberResponse>>;
