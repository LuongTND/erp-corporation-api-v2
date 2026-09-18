namespace Application;

public sealed record GetInterviewSchedulesQuery(Guid CandidateId) : IRequest<IEnumerable<InterviewScheduleResponse>>;
