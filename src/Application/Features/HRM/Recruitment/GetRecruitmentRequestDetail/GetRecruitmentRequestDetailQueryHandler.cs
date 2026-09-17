namespace Application;

public sealed class GetRecruitmentRequestDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRecruitmentRequestDetailQuery, RecruitmentRequestDetailResponse>
{
    public async Task<RecruitmentRequestDetailResponse> Handle(GetRecruitmentRequestDetailQuery q, CancellationToken ct)
    {
        var r = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(x => x.Id == q.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", q.RequestId));

        var creator = await unitOfWork.Repository<User>().FindAsync(u => u.Id == r.RequestedByUserId, ct);
        var storeName = r.StoreId.HasValue
            ? (await unitOfWork.Repository<Store>().FindAsync(s => s.Id == r.StoreId.Value, ct))?.Name
            : null;
        var deptName = r.DepartmentId.HasValue
            ? (await unitOfWork.Repository<Department>().FindAsync(d => d.Id == r.DepartmentId.Value, ct))?.DepartmentName
            : null;

        var postings = await unitOfWork.Repository<JobPosting>()
            .GetPagedAsync(new QueryInfo { Top = 100, Skip = 0, NeedTotalCount = false }, filter: p => p.RecruitmentRequestId == r.Id, ct: ct);

        var approvalHistory = r.WorkflowInstanceId.HasValue
            ? await (from t in unitOfWork.Repository<WorkflowTask>().Query()
                     join u in unitOfWork.Repository<User>().Query() on t.AssignedTo equals u.Id into uj
                     from u in uj.DefaultIfEmpty()
                     join rol in unitOfWork.Repository<Role>().Query() on t.AssignedToRoleId equals rol.Id into rj
                     from rol in rj.DefaultIfEmpty()
                     join actor in unitOfWork.Repository<User>().Query() on t.ActedByUserId equals actor.Id into aj
                     from actor in aj.DefaultIfEmpty()
                     where t.InstanceId == r.WorkflowInstanceId.Value
                     orderby t.StepOrder
                     select new WorkflowTaskResponse
                     {
                         Id = t.Id,
                         InstanceId = t.InstanceId,
                         EntityType = "RecruitmentRequest",
                         EntityId = r.Id,
                         StepOrder = t.StepOrder,
                         StepName = t.StepName,
                         AssignedTo = t.AssignedTo,
                         AssignedToRoleId = t.AssignedToRoleId,
                         AssignedToName = u != null ? u.FullName : (rol != null ? rol.RoleName : string.Empty),
                         ActedByUserId = t.ActedByUserId,
                         ActedByName = actor != null ? actor.FullName : string.Empty,
                         Status = t.Status.ToString(),
                         Note = t.Note,
                         ActedAt = t.ActedAt,
                         CreatedAt = t.CreatedAt,
                     }).ToListAsync(ct)
            : [];

        return new RecruitmentRequestDetailResponse
        {
            Id = r.Id,
            RequestCode = r.RequestCode,
            RequestContext = r.RequestContext.ToString(),
            DepartmentId = r.DepartmentId,
            StoreId = r.StoreId,
            PositionTitle = r.PositionTitle,
            RequestedByUserId = r.RequestedByUserId,
            RequestedByName = creator?.FullName ?? string.Empty,
            StoreName = storeName,
            DepartmentName = deptName,
            Headcount = r.Headcount,
            Reason = r.Reason,
            JobDescription = r.JobDescription,
            RequiredByDate = r.RequiredByDate,
            Status = r.Status.ToString(),
            RejectionNote = r.RejectionNote,
            NeedMoreInfoNote = r.NeedMoreInfoNote,
            CancelNote = r.CancelNote,
            WorkflowInstanceId = r.WorkflowInstanceId,
            ApprovalHistory = approvalHistory,
            CreatedAt = r.CreatedAt,
            JobPostings = postings.Items.Select(p => new JobPostingResponse
            {
                Id = p.Id,
                RecruitmentRequestId = p.RecruitmentRequestId,
                Title = p.Title,
                Channel = p.Channel.ToString(),
                PostUrl = p.PostUrl,
                EstimatedCost = p.EstimatedCost,
                CostStatus = p.CostStatus.ToString(),
                CostApprovedByUserId = p.CostApprovedByUserId,
                CostApprovedAt = p.CostApprovedAt,
                CostRejectionNote = p.CostRejectionNote,
                PostedAt = p.PostedAt,
                ExpiresAt = p.ExpiresAt,
                CreatedAt = p.CreatedAt
            })
        };
    }
}
