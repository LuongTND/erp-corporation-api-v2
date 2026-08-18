namespace Contract;

public sealed class EmploymentContractResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal Salary { get; set; }
    public decimal? SalaryForSocialInsurance { get; set; }
    public string? PositionTitle { get; set; }
    public string? FileUrl { get; set; }
    public DateOnly? SignedDate { get; set; }
    public string? TerminationReason { get; set; }
    public Guid? RenewedFromContractId { get; set; }
    public Guid? TemplateId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsActive => Status == "Active";
}

public sealed class ContractSalaryComparisonResponse
{
    public Guid UserId { get; set; }
    public Guid? ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public decimal? ContractSalary { get; set; }
    public decimal? ActualHourlyRate { get; set; }
    public bool HasActiveContract { get; set; }
    public bool HasSalaryRecord { get; set; }
}
