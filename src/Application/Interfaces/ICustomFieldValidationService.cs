namespace Application;

public interface ICustomFieldValidationService
{
    /// <summary>
    /// Validates a custom field value against its definition rules.
    /// Returns null if valid, error message if invalid.
    /// </summary>
    string? Validate(CustomFieldDefinition definition, string? value);
}
