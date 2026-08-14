namespace Application;

public sealed record GetStoreMembersQuery(Guid StoreId) : IRequest<IEnumerable<StoreMemberResponse>>;
