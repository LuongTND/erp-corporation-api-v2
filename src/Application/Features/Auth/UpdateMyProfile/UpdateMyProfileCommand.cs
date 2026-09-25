namespace Application;

public sealed record UpdateMyProfileCommand(
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
    // Financial — không bao gồm TaxCode/SocialInsuranceCode (HR cấp), chỉ bank
    string? TaxCode,
    string? SocialInsuranceCode,
    string? BankName,
    string? BankAccountNumber,
    string? BankBranch
) : IRequest<Unit>;
