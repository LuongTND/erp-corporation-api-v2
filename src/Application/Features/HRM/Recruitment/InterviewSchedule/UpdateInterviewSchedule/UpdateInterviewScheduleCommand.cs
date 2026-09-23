namespace Application;

public sealed record UpdateInterviewScheduleCommand(
    DateTimeOffset ScheduledAt,
    string? LocationNote,
    string? Notes,
    Guid? InterviewerId,
    InterviewScheduleStatus Status
) : IRequest<Unit>
{
    public Guid ScheduleId { get; init; }
}
