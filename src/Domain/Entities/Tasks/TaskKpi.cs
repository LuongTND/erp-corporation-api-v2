namespace Domain;

public class TaskKpi : EntityBase<Guid>
{
    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    // Cross-module reference — KPI module quản lý entity này
    public Guid KpiId { get; set; }

    private decimal? _weight;

    // Weight theo thang 0-1 (0% → 1.0 = 100%)
    public decimal? Weight
    {
        get => _weight;
        set => _weight = value is null ? null : Math.Clamp(value.Value, 0m, 1m);
    }
}
