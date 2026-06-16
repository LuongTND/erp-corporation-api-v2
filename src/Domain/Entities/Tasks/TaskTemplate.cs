using Domain.Base;
using Domain.Enums.Tasks;

namespace Domain.Entities.Tasks;

public class TaskTemplate : BaseEntity
{
    public string TemplateName { get; private set; } = null!;
    public string? Description { get; private set; }
    public TaskPriority? DefaultPriority { get; private set; }
    public int? DefaultDurationDays { get; private set; }
    public Guid CreatedBy { get; private set; }

    private TaskTemplate() : base()
    {
    }

    public static TaskTemplate Create(
        string templateName,
        string? description,
        TaskPriority? defaultPriority,
        int? defaultDurationDays,
        Guid createdBy)
    {
        return new TaskTemplate
        {
            TemplateName = templateName.Trim(),
            Description = description,
            DefaultPriority = defaultPriority,
            DefaultDurationDays = defaultDurationDays,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string templateName,
        string? description,
        TaskPriority? defaultPriority,
        int? defaultDurationDays)
    {
        TemplateName = templateName.Trim();
        Description = description;
        DefaultPriority = defaultPriority;
        DefaultDurationDays = defaultDurationDays;
    }
}
