namespace Domain;

public class InterviewRuleConfigStep : EntityBase<Guid>
{
    public Guid InterviewRuleConfigId { get; set; }
    public InterviewRuleConfig? InterviewRuleConfig { get; set; }

    public int RoundNumber { get; set; }

    public string Label { get; set; } = string.Empty;

    public string InterviewerRoleKey { get; set; } = string.Empty;

    public string SchedulerRoleKey { get; set; } = string.Empty;

    public InterviewLocation Location { get; set; }
}
