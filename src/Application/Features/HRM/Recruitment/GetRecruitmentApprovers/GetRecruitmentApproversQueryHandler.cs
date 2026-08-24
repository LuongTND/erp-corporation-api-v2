namespace Application;

public sealed class GetRecruitmentApproversQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRecruitmentApproversQuery, IEnumerable<RecruitmentApproverConfigResponse>>
{
    public async Task<IEnumerable<RecruitmentApproverConfigResponse>> Handle(
        GetRecruitmentApproversQuery _, CancellationToken ct)
    {
        var configs = await unitOfWork.Repository<RecruitmentApproverConfig>()
            .Query()
            .Include(c => c.Approver)
            .Include(c => c.Department)
            .ToListAsync(ct);

        return configs.Select(c => new RecruitmentApproverConfigResponse
        {
            Id             = c.Id,
            ApproverId     = c.ApproverId,
            ApproverName   = c.Approver?.FullName ?? string.Empty,
            DepartmentId   = c.DepartmentId,
            DepartmentName = c.Department?.DepartmentName,
            Note           = c.Note,
        });
    }
}
