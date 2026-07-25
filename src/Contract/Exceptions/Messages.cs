namespace Contract;

public static class ExceptionMessages
{
    public const string Forbidden = "You do not have permission to access this resource";
    public const string DatabaseOperationFailed = "Database operation failed";
    public const string InternalError = "Unexpected error occurred";

    public static string NotFound(string entityName, object id) => $"{entityName} with ID {id} was not found";
    public static string NotFoundField(string entityName, string fieldName, object value) => $"{entityName} with {fieldName} '{value}' was not found";
    public static string NotExists(string entityName, object value) => $"{entityName} with value '{value}' does not exist";
    public static string AlreadyExists(string entityName, object value) => $"{entityName} with value '{value}' already exists";
    public static string Invalid(string entityName) => $"{entityName} is invalid";
}

public static class BusinessMessages
{
    public static string CreatedSuccessfully(string entityName) => $"Created {entityName} successfully";
    public static string UpdatedSuccessfully(string entityName) => $"Updated {entityName} successfully";
    public static string DeletedSuccessfully(string entityName) => $"Deleted {entityName} successfully";
    public static string FoundSuccessfully(string entityName) => $"Found {entityName} successfully";
    public static string CreateFailure(string entityName) => $"Failed to create {entityName}";
    public static string UpdateFailure(string entityName) => $"Failed to update {entityName}";
    public static string DeleteFailure(string entityName) => $"Failed to delete {entityName}";
    public static string GetFailure(string entityName) => $"Failed to get {entityName}";
}

public static class ValidationMessages
{
    public const string Required = "{PropertyName} is required";
    public const string InvalidFormat = "{PropertyName} has an invalid format";
    public const string InvalidEnumValue = "{PropertyName} has an invalid enum value";
    public const string InvalidEmail = "{PropertyName} is not a valid email format";
    public const string InvalidPhoneNumber = "{PropertyName} is not a valid phone number";
    public const string InvalidDate = "{PropertyName} is not a valid date";
    public const string InvalidPlateNumberFormat = "{PropertyName} can only contain letters, numbers, and hyphens";
    public const string Expired = "{PropertyName} has expired";
    public const string MinLength = "{PropertyName} must be at least {MinLength} characters";
    public const string MaxLength = "{PropertyName} must not exceed {MaxLength} characters";
    public const string ExactLength = "{PropertyName} must be exactly {ExactLength} characters";
    public const string Range = "{PropertyName} must be between {MinLength} and {MaxLength} characters";
    public const string GreaterThan = "{PropertyName} must be greater than {ComparisonValue}";
    public const string GreaterThanOrEqual = "{PropertyName} must be greater than or equal to {ComparisonValue}";
    public const string LessThan = "{PropertyName} must be less than {ComparisonValue}";
    public const string LessThanOrEqual = "{PropertyName} must be less than or equal to {ComparisonValue}";
    public const string OnlyLetters = "{PropertyName} can only contain letters";
    public const string OnlyNumbers = "{PropertyName} can only contain numbers";
    public const string OnlyAlphanumeric = "{PropertyName} can only contain letters and numbers";
    public const string NotContainSpaces = "{PropertyName} must not contain spaces";
    public const string ProhibitedContent = "Your comment contains prohibited words";
    public const string ListNotEmpty = "{PropertyName} must not be empty";
    public const string ListMinItems = "{PropertyName} must contain at least {MinItems} items";
    public const string ListMaxItems = "{PropertyName} must contain no more than {MaxItems} items";
    public const string MustBeTrue = "{PropertyName} must be true";
    public const string MustBeFalse = "{PropertyName} must be false";
    public const string MustMatch = "{PropertyName} must match {ComparisonProperty}";
    public const string MustNotMatch = "{PropertyName} must not be the same as {ComparisonProperty}";
}
