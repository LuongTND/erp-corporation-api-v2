namespace Domain;

public class RegionHours : AuditableEntityBase<Guid>
{
    public Guid RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
