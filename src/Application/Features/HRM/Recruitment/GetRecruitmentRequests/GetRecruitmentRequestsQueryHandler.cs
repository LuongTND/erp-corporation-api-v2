namespace Application;

public sealed class GetRecruitmentRequestsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRecruitmentRequestsQuery, QueryResult<RecruitmentRequestResponse>>
{
    public async Task<QueryResult<RecruitmentRequestResponse>> Handle(
        GetRecruitmentRequestsQuery q, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<RecruitmentRequest>()
            .GetPagedAsync(
                q.QueryInfo,
                filter: r =>
                    (!q.Status.HasValue || r.Status == q.Status.Value) &&
                    (!q.RequestContext.HasValue || r.RequestContext == q.RequestContext.Value) &&
                    (!q.DepartmentId.HasValue || r.DepartmentId == q.DepartmentId.Value) &&
                    (!q.StoreId.HasValue || r.StoreId == q.StoreId.Value),
                ct: ct);

        return new QueryResult<RecruitmentRequestResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(r => new RecruitmentRequestResponse
            {
                Id = r.Id,
                RequestContext = r.RequestContext.ToString(),
                DepartmentId = r.DepartmentId,
                DepartmentName = r.Department?.DepartmentName,
                StoreId = r.StoreId,
                PositionTitle = r.PositionTitle,
                RequestedByUserId = r.RequestedByUserId,
                RequestedByName = r.RequestedBy?.FullName ?? string.Empty,
                Headcount = r.Headcount,
                Reason = r.Reason,
                JobDescription = r.JobDescription,
                RequiredByDate = r.RequiredByDate,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            })
        };
    }
}
