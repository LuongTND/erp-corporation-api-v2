namespace Domain;

public class StoreHours : AuditableEntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; }
}
