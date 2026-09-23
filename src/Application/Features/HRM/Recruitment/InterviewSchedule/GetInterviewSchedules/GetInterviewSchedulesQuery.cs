namespace Application;

public sealed record GetInterviewSchedulesQuery(
    Guid JobPostingId,
    DateOnly? From,
    DateOnly? To,
    Guid? InterviewerId
) : IRequest<IEnumerable<InterviewScheduleListItemResponse>>;
