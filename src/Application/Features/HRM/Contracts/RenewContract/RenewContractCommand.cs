namespace Application;

public sealed record RenewContractCommand(
    Guid UserId,
    Guid ContractId,
    ContractType Type,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal Salary,
    decimal? SalaryForSocialInsurance,
    string? PositionTitle,
    DateOnly? SignedDate,
    Stream FileStream,
    string OriginalFileName,
    string ContentType
) : IRequest<Guid>;
