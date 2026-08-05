namespace Application;

public sealed record CreateEmployeeCommand(
    string FullName,
    string Email,
    Guid JobLevelId,
    DateOnly DateOfJoin,
    string? EmployeeCode = null,
    Gender? Gender = null,
    DateOnly? DateOfBirth = null,
    string? IdentityCardNumber = null,
    DateOnly? IdentityCardIssuedDate = null,
    string? IdentityCardIssuedPlace = null,
    string? PhoneNumber = null,
    string? PermanentAddress = null,
    string? CurrentAddress = null,
    string? TaxCode = null,
    string? SocialInsuranceCode = null,
    Guid? ManagerId = null,
    string? AvatarUrl = null
) : IRequest<Guid>;
