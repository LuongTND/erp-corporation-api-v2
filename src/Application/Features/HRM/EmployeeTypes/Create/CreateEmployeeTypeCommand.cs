namespace Application;

public sealed record CreateEmployeeTypeCommand(
    string Name,
    string Code,
    string? Description
) : IRequest<Guid>;
