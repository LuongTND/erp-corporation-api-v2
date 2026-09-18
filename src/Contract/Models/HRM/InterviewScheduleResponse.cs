namespace Contract;

public class InterviewScheduleResponse
{
    public Guid Id { get; init; }
    public Guid CandidateId { get; init; }
    public string CandidateName { get; init; } = string.Empty;
    public Guid InterviewerId { get; init; }
    public string InterviewerName { get; init; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; init; }
    public string Location { get; init; } = string.Empty;
    public string? LocationNote { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string? InterviewResult { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
