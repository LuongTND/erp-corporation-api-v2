namespace Application;

public sealed class ResolveInterviewRuleQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ResolveInterviewRuleQuery, InterviewRuleConfigResponse?>
{
    public async Task<InterviewRuleConfigResponse?> Handle(ResolveInterviewRuleQuery q, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == q.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", q.ApplicationId));

        var request = application.RecruitmentRequestId.HasValue
            ? await unitOfWork.Repository<RecruitmentRequest>()
                .FindAsync(r => r.Id == application.RecruitmentRequestId.Value, ct)
            : null;

        if (request == null)
            return null;

        var queryInfo = new QueryInfo { Top = 50, Skip = 0, NeedTotalCount = false };
        var rules = await unitOfWork.Repository<Domain.InterviewRuleConfig>()
            .GetPagedAsync(queryInfo,
                filter: r => r.IsActive && r.Context == request.RequestContext,
                ct: ct);

        var sorted = rules.Items.OrderByDescending(r => r.Priority).ToList();

        Domain.InterviewRuleConfig? matched;
        if (request.RequestContext == RecruitmentRequestContext.Store)
        {
            var regionId = await GetRegionIdAsync(request, unitOfWork, ct);
            matched = sorted.FirstOrDefault(r => r.RegionId != null && r.RegionId == regionId)
                   ?? sorted.FirstOrDefault(r => r.RegionId == null);
        }
        else
        {
            matched = sorted.FirstOrDefault(r => r.DepartmentId != null && r.DepartmentId == request.DepartmentId)
                   ?? sorted.FirstOrDefault(r => r.DepartmentId == null);
        }

        if (matched == null) return null;

        return matched.Adapt<InterviewRuleConfigResponse>();
    }

    private static async Task<Guid?> GetRegionIdAsync(RecruitmentRequest request, IUnitOfWork uow, CancellationToken ct)
    {
        if (!request.StoreId.HasValue) return null;
        var store = await uow.Repository<Store>().FindAsync(s => s.Id == request.StoreId.Value, ct);
        return store?.RegionId;
    }
}
