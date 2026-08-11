namespace Application;

public sealed record UpdateEmployeeCommand(
    Guid UserId,
    string FullName,
    Guid? JobLevelId,
    Guid? ManagerId,
    // Profile
    Gender? Gender,
    DateOnly? DateOfBirth,
    string? PhoneNumber,
    string? PermanentAddress,
    string? CurrentAddress,
    // Identity
    string? IdentityCardNumber,
    DateOnly? IdentityCardIssuedDate,
    string? IdentityCardIssuedPlace,
    string? PassportNumber,
    DateOnly? PassportExpiryDate,
    // Employment
    DateOnly? DateOfJoin,
    ContractType? ContractType,
    string? TaxCode,
    string? SocialInsuranceCode,
    string? BankName,
    string? BankAccountNumber,
    string? BankBranch
) : IRequest<Unit>;
