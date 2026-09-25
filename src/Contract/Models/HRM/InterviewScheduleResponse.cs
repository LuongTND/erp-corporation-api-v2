namespace Contract;

public sealed class InterviewScheduleSummaryResponse
{
    public Guid Id { get; init; }
    public DateTimeOffset ScheduledAt { get; init; }
    public string? LocationNote { get; init; }
    public string? Notes { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid? InterviewerId { get; init; }
    public string? InterviewerName { get; init; }
    public string? InterviewResult { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
}

public sealed class InterviewScheduleListItemResponse
{
    public Guid Id { get; init; }
    public Guid ApplicationId { get; init; }
    public string ApplicantName { get; init; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; init; }
    public string? LocationNote { get; init; }
    public string? Notes { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid? InterviewerId { get; init; }
    public string? InterviewerName { get; init; }
}
