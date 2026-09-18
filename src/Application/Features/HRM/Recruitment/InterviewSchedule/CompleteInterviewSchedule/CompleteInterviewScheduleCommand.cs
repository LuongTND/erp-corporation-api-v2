namespace Application;

public sealed record CompleteInterviewScheduleCommand(
    Guid ScheduleId,
    string InterviewResult
) : IRequest<Unit>;
