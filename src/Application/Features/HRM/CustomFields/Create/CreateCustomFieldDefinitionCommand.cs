namespace Application;

public sealed record CreateCustomFieldDefinitionCommand(
    string Code,
    string Name,
    CustomFieldType FieldType,
    string Module,
    bool IsRequired = false,
    int SortOrder = 0,
    string? Placeholder = null,
    string? HelpText = null,
    string? Group = null,
    string? ValidationJson = null,
    IEnumerable<CreateCustomFieldOptionDto>? Options = null
) : IRequest<Guid>;

public sealed record CreateCustomFieldOptionDto(
    string Value,
    string Label,
    int SortOrder = 0
);
