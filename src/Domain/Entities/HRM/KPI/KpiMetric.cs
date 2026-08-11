namespace Domain;

public class KpiMetric : EntityBase<Guid>
{
    public Guid TemplateId { get; set; }
    public KpiTemplate Template { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Target { get; set; }
    public MetricType Type { get; set; }
}
