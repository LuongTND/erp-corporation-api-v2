using System.Text.Json;
using System.Text.RegularExpressions;

namespace Infrastructure;

[RegisterService(typeof(ICustomFieldValidationService))]
public sealed class CustomFieldValidationService : ICustomFieldValidationService
{
    // Internal regex map — admin never sees these, they pick a ValidationType enum
    private static readonly Dictionary<ValidationType, string> RegexMap = new()
    {
        [ValidationType.Email]       = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        [ValidationType.Phone]       = @"^(0|\+84)[3-9]\d{8}$",
        [ValidationType.CitizenId]   = @"^\d{12}$",
        [ValidationType.TaxCode]     = @"^\d{10}(-\d{3})?$",
        [ValidationType.Passport]    = @"^[A-Z]\d{7,8}$",
        [ValidationType.EmployeeCode]= @"^NV\d{4}$",
    };

    public string? Validate(CustomFieldDefinition definition, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return definition.IsRequired ? $"{definition.Name} là bắt buộc." : null;

        var opts = ParseOptions(definition.ValidationJson);
        if (opts is null)
            return null;

        return definition.FieldType switch
        {
            CustomFieldType.Text or CustomFieldType.TextArea => ValidateText(definition.Name, value, opts),
            CustomFieldType.Number => ValidateNumber(definition.Name, value, opts),
            CustomFieldType.Date => ValidateDate(definition.Name, value, opts),
            _ => null,
        };
    }

    private static string? ValidateText(string name, string value, CustomFieldValidationOptions opts)
    {
        if (opts.MinLength.HasValue && value.Length < opts.MinLength)
            return $"{name} phải có ít nhất {opts.MinLength} ký tự.";

        if (opts.MaxLength.HasValue && value.Length > opts.MaxLength)
            return $"{name} không được vượt quá {opts.MaxLength} ký tự.";

        if (opts.ValidationType.HasValue && RegexMap.TryGetValue(opts.ValidationType.Value, out var pattern))
        {
            if (!Regex.IsMatch(value, pattern))
                return $"{name} không đúng định dạng.";
        }

        return null;
    }

    private static string? ValidateNumber(string name, string value, CustomFieldValidationOptions opts)
    {
        if (!double.TryParse(value, out var number))
            return $"{name} phải là số.";

        if (opts.Min.HasValue && number < opts.Min)
            return $"{name} phải lớn hơn hoặc bằng {opts.Min}.";

        if (opts.Max.HasValue && number > opts.Max)
            return $"{name} phải nhỏ hơn hoặc bằng {opts.Max}.";

        return null;
    }

    private static string? ValidateDate(string name, string value, CustomFieldValidationOptions opts)
    {
        if (!DateOnly.TryParse(value, out var date))
            return $"{name} phải là ngày hợp lệ (yyyy-MM-dd).";

        if (opts.MinDate.HasValue && date < opts.MinDate)
            return $"{name} phải từ {opts.MinDate:yyyy-MM-dd} trở đi.";

        if (opts.MaxDate.HasValue && date > opts.MaxDate)
            return $"{name} phải trước {opts.MaxDate:yyyy-MM-dd}.";

        return null;
    }

    private static CustomFieldValidationOptions? ParseOptions(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<CustomFieldValidationOptions>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }
}
