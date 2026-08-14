namespace Application;

public sealed record RemoveStoreMemberCommand(Guid StoreId, Guid UserId) : IRequest<Unit>;
