namespace Application;

public sealed class GetInterviewSchedulesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInterviewSchedulesQuery, IEnumerable<InterviewScheduleResponse>>
{
    public async Task<IEnumerable<InterviewScheduleResponse>> Handle(
        GetInterviewSchedulesQuery q, CancellationToken ct)
    {
        var queryInfo = new QueryInfo { Top = 50, Skip = 0, NeedTotalCount = false };
        var result = await unitOfWork.Repository<Domain.InterviewSchedule>()
            .GetPagedAsync(queryInfo, filter: s => s.CandidateId == q.CandidateId, ct: ct);

        return result.Items.OrderByDescending(s => s.ScheduledAt)
            .Select(s => s.Adapt<InterviewScheduleResponse>());
    }
}
