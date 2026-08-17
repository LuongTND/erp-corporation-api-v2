namespace Contract;

public sealed class KpiMetricResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Target { get; set; }
    public string Type { get; set; } = string.Empty;
}
