namespace Application;

public sealed record UpdateCustomFieldDefinitionCommand(
    Guid DefinitionId,
    string Name,
    bool IsRequired,
    bool IsActive,
    int SortOrder,
    string? Placeholder = null,
    string? HelpText = null,
    string? Group = null,
    string? ValidationJson = null,
    IEnumerable<UpsertCustomFieldOptionDto>? Options = null
) : IRequest<Unit>;

public sealed record UpsertCustomFieldOptionDto(
    Guid? Id,
    string Value,
    string Label,
    int SortOrder,
    bool IsActive = true
);
