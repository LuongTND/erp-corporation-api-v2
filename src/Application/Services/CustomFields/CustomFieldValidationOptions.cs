namespace Application;

public sealed class CustomFieldValidationOptions
{
    public ValidationType? ValidationType { get; init; }
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public double? Min { get; init; }
    public double? Max { get; init; }
    public DateOnly? MinDate { get; init; }
    public DateOnly? MaxDate { get; init; }
}
