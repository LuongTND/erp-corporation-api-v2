namespace Application;

public sealed record AddStoreMemberCommand(Guid StoreId, Guid UserId, DateOnly StartDate, bool IsHomeStore) : IRequest<Guid>;
