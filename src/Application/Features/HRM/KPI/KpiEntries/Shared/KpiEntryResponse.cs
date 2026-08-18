namespace Application;

public sealed class KpiEntryResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid KpiMetricId { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal ActualValue { get; set; }
    public decimal Score { get; set; }
    public string? Note { get; set; }
}
