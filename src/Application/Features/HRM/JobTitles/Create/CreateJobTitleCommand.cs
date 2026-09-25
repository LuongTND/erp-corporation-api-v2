namespace Application;

public sealed record CreateJobTitleCommand(
    string Code,
    string Name,
    string? Description,
    JobTitleLevel Level,
    JobTitleUnitType UnitType
) : IRequest<Guid>;
