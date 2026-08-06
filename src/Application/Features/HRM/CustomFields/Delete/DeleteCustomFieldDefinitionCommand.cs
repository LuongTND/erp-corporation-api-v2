namespace Application;

public sealed record DeleteCustomFieldDefinitionCommand(Guid DefinitionId) : IRequest<Unit>;
