namespace Application;

public sealed record CreateInterviewScheduleCommand(
    Guid CandidateId,
    Guid InterviewerId,
    DateTimeOffset ScheduledAt,
    InterviewLocation Location,
    string? LocationNote,
    string? Notes
) : IRequest<Guid>;
