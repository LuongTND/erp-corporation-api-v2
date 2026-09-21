namespace Application;

public sealed record UpdateRoundTypeCommand(
    Guid Id,
    string Name,
    int DisplayOrder
) : IRequest<Unit>;
