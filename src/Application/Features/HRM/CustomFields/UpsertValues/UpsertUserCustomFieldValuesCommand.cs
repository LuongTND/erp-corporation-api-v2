namespace Application;

public sealed record UpsertUserCustomFieldValuesCommand(
    Guid UserId,
    IEnumerable<CustomFieldValueInput> Values
) : IRequest<Unit>;

public sealed record CustomFieldValueInput(Guid DefinitionId, string Value);
