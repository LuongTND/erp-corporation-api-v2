namespace Application;

public sealed record UpdateJobTitleCommand(
    Guid JobTitleId,
    string Code,
    string Name,
    string? Description,
    JobTitleLevel Level,
    JobTitleUnitType UnitType
) : IRequest<Unit>;
