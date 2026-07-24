namespace Domain;

public class TaskTemplate : AuditableEntityBase<Guid>
{
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DefaultPriorityId { get; set; }
    public TaskPriority? DefaultPriority { get; set; }
    public int? DefaultDurationDays { get; set; }
    public bool IsActive { get; set; } = true;
}
