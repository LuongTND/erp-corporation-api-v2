namespace Contract;

public sealed class PayrollEntryResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal HourlyRateSnapshot { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal GrossPay { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public decimal? SocialInsurance { get; set; }
    public decimal? HealthInsurance { get; set; }
    public decimal? UnemploymentIns { get; set; }
    public decimal? PersonalIncomeTax { get; set; }
    public string? Note { get; set; }
}
