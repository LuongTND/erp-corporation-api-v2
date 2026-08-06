namespace Application;

public sealed record GetCustomFieldDefinitionsQuery(string? Module = null) : IRequest<IEnumerable<CustomFieldDefinitionResponse>>;
