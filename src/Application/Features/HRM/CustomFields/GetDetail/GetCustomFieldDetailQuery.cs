namespace Application;

public sealed record GetCustomFieldDetailQuery(Guid Id) : IRequest<CustomFieldDefinitionResponse>;
