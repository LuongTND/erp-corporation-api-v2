namespace Application;

public sealed record CreateRoundTypeCommand(
    string Name,
    int DisplayOrder
) : IRequest<Guid>;
