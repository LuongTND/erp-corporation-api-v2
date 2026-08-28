namespace Domain;

public class InterviewSchedule : AuditableEntityBase<Guid>
{
    public Guid CandidateId { get; set; }
    public Candidate? Candidate { get; set; }

    public Guid InterviewerId { get; set; }
    public User? Interviewer { get; set; }

    public DateTimeOffset ScheduledAt { get; set; }

    public InterviewLocation Location { get; set; }

    public string? LocationNote { get; set; }

    public InterviewScheduleStatus Status { get; set; } = InterviewScheduleStatus.Scheduled;

    public string? Notes { get; set; }

    // điền sau khi phỏng vấn xong
    public string? InterviewResult { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
