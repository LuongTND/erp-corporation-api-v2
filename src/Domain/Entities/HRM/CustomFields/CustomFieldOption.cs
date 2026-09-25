namespace Domain;

public class CustomFieldOption : EntityBase<Guid>
{
    public Guid DefinitionId { get; set; }
    public CustomFieldDefinition? Definition { get; set; }

    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
