namespace Domain;

public class PayrollEntry : AuditableEntityBase<Guid>
{
    public Guid PayrollRunId { get; set; }
    public PayrollRun PayrollRun { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Snapshot tại thời điểm tạo run — không reference SalaryRecord live
    public decimal HourlyRateSnapshot { get; set; }
    public decimal HoursWorked { get; set; }

    public decimal GrossPay { get; set; }       // HourlyRateSnapshot × HoursWorked
    public decimal BonusAmount { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }          // GrossPay + BonusAmount - TotalDeductions

    // Deduction breakdown (nullable — điền sau khi rõ nghiệp vụ BHXH/PIT)
    public decimal? SocialInsurance { get; set; }
    public decimal? HealthInsurance { get; set; }
    public decimal? UnemploymentIns { get; set; }
    public decimal? PersonalIncomeTax { get; set; }

    public string? Note { get; set; }
}
