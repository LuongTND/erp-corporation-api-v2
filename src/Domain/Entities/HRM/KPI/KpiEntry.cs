namespace Domain;

public class KpiEntry : AuditableEntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid KpiMetricId { get; set; }
    public KpiMetric KpiMetric { get; set; } = null!;

    public int Month { get; set; }
    public int Year { get; set; }

    public decimal ActualValue { get; set; }
    public decimal Score { get; set; }   // 0-100, HR nhập trực tiếp
    public string? Note { get; set; }
}
