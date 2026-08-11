namespace Application;

public sealed record UpdatePayrollEntryCommand(
    Guid EntryId,
    decimal HoursWorked,
    decimal BonusAmount,
    decimal? SocialInsurance,
    decimal? HealthInsurance,
    decimal? UnemploymentIns,
    decimal? PersonalIncomeTax,
    string? Note) : IRequest;
