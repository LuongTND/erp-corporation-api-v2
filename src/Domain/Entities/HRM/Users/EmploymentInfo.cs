namespace Domain;

public class EmploymentInfo : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public DateOnly DateOfJoin { get; set; }
    public ContractType? ContractType { get; set; }
    public string? TaxCode { get; set; }
    public string? SocialInsuranceCode { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankBranch { get; set; }
    public DateTimeOffset? ResignedAt { get; set; }
    public bool? HandoverCompleted { get; set; }
}
