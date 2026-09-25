namespace Application;

public sealed record CreateContractCommand(
    Guid UserId,
    ContractType Type,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal Salary,
    decimal? SalaryForSocialInsurance,
    string? PositionTitle,
    DateOnly? SignedDate,
    Guid? TemplateId,
    Stream FileStream,
    string OriginalFileName,
    string ContentType
) : IRequest<Guid>;
