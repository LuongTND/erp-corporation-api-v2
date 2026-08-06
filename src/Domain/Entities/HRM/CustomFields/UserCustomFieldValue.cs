namespace Domain;

public class UserCustomFieldValue : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid DefinitionId { get; set; }
    public CustomFieldDefinition? Definition { get; set; }

    public string Value { get; set; } = string.Empty;
}
