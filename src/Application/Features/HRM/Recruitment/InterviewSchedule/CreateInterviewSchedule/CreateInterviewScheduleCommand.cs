namespace Application;

public sealed record CreateInterviewScheduleCommand(
    Guid ApplicationId,
    DateTimeOffset ScheduledAt,
    string? LocationNote,
    string? Notes,
    Guid? InterviewerId
) : IRequest<Guid>;
