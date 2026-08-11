namespace Application;

public sealed record CreateBonusPolicyCommand(
    string Name,
    string? Description
) : IRequest<Guid>;
