namespace Application;

public sealed record CreateInterviewScheduleCommand(
    Guid ApplicationId,
    Guid InterviewerId,
    DateTimeOffset ScheduledAt,
    InterviewLocation Location,
    string? LocationNote,
    string? Notes
) : IRequest<Guid>;
