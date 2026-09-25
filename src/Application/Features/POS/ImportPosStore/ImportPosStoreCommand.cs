namespace Application;

public sealed record ImportPosStoreCommand(
    Guid PosStoreId,
    Guid? ManagerId
) : IRequest<Guid>;
