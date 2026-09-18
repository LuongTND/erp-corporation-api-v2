namespace Application;

public sealed record GetInterviewSchedulesQuery(Guid ApplicationId) : IRequest<IEnumerable<InterviewScheduleResponse>>;
