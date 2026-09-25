namespace Domain;

public class EmploymentContract : AuditableEntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string ContractNumber { get; set; } = null!;          // HD-{YYYY}-{NNNN}, unique global
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }                       // null = không xác định thời hạn
    public decimal Salary { get; set; }                          // lương ghi trên HĐ
    public decimal? SalaryForSocialInsurance { get; set; }       // lương đóng BHXH, null → dùng Salary
    public string? PositionTitle { get; set; }
    public string? FileUrl { get; set; }
    public DateOnly? SignedDate { get; set; }
    public string? TerminationReason { get; set; }

    public Guid? RenewedFromContractId { get; set; }
    public EmploymentContract? RenewedFromContract { get; set; }

    public Guid? TemplateId { get; set; }
    public ContractTemplate? Template { get; set; }
}
