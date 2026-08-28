namespace Application;

public sealed class GetInterviewRuleConfigsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInterviewRuleConfigsQuery, IEnumerable<InterviewRuleConfigResponse>>
{
    public async Task<IEnumerable<InterviewRuleConfigResponse>> Handle(
        GetInterviewRuleConfigsQuery q, CancellationToken ct)
    {
        var queryInfo = new QueryInfo { Top = 200, Skip = 0, NeedTotalCount = false };
        var result = await unitOfWork.Repository<Domain.InterviewRuleConfig>()
            .GetPagedAsync(queryInfo,
                filter: r =>
                    (q.Context == null || r.Context == q.Context) &&
                    (q.IsActive == null || r.IsActive == q.IsActive),
                ct: ct);

        return result.Items.OrderByDescending(r => r.Priority).Select(r => new InterviewRuleConfigResponse
        {
            Id = r.Id,
            Name = r.Name,
            Context = r.Context.ToString(),
            RegionId = r.RegionId,
            RegionName = r.Region?.Name,
            DepartmentId = r.DepartmentId,
            DepartmentName = r.Department?.DepartmentName,
            InterviewerRoleKey = r.InterviewerRoleKey,
            Location = r.Location.ToString(),
            SchedulerRoleKey = r.SchedulerRoleKey,
            NotifyRoleKey = r.NotifyRoleKey,
            Priority = r.Priority,
            IsActive = r.IsActive,
        });
    }
}
