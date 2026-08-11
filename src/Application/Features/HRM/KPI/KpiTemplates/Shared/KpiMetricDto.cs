namespace Application;

public sealed record KpiMetricDto(
    string Name,
    string Unit,
    decimal Weight,
    decimal Target,
    MetricType Type
);
