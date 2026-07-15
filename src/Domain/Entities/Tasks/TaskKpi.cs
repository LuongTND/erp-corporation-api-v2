namespace Domain;
public class TaskKpi
{
    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public Guid KPIID { get; private set; } // Liên kết logic với bảng KPI

    public decimal? Weight { get; private set; }

    private TaskKpi()
    {
    }

    public static TaskKpi Create(Guid taskId, Guid kpiId, decimal? weight = null)
    {
        return new TaskKpi
        {
            TaskID = taskId,
            KPIID = kpiId,
            Weight = weight
        };
    }
}
