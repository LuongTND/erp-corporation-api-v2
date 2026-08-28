namespace Application;

public sealed record CancelInterviewScheduleCommand(Guid ScheduleId, string? Reason) : IRequest<Unit>;
