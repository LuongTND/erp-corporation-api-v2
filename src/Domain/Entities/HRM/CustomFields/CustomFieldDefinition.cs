namespace Domain;

public class CustomFieldDefinition : AuditableEntityBase<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CustomFieldType FieldType { get; set; }
    public string Module { get; set; } = string.Empty;

    public bool IsSystem { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? Group { get; set; }
    public string? ValidationJson { get; set; }

    public ICollection<CustomFieldOption> Options { get; set; } = [];
    public ICollection<UserCustomFieldValue> Values { get; set; } = [];
}
