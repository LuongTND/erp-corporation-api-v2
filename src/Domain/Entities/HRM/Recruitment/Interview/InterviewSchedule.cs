namespace Domain;

public class InterviewSchedule : AuditableEntityBase<Guid>
{
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }

    public Guid InterviewerId { get; set; }
    public User? Interviewer { get; set; }

    // vòng phỏng vấn, khớp với InterviewRuleConfigStep.RoundNumber
    public int Round { get; set; } = 1;

    public DateTimeOffset ScheduledAt { get; set; }

    public InterviewLocation Location { get; set; }

    public string? LocationNote { get; set; }

    public InterviewScheduleStatus Status { get; set; } = InterviewScheduleStatus.Scheduled;

    public string? Notes { get; set; }

    // điền sau khi phỏng vấn xong
    public string? InterviewResult { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
