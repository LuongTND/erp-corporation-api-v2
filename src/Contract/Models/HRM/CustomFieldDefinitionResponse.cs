namespace Contract;

public sealed class CustomFieldDefinitionResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string FieldType { get; init; } = string.Empty;
    public string Module { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
    public bool IsRequired { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
    public string? Placeholder { get; init; }
    public string? HelpText { get; init; }
    public string? Group { get; init; }
    public string? ValidationJson { get; init; }
    public IEnumerable<CustomFieldOptionResponse> Options { get; init; } = [];
}

public sealed class CustomFieldOptionResponse
{
    public Guid Id { get; init; }
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}
