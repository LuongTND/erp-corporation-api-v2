namespace Application;

public sealed class GetWorkflowTemplatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWorkflowTemplatesQuery, IReadOnlyList<WorkflowTemplateResponse>>
{
    public async Task<IReadOnlyList<WorkflowTemplateResponse>> Handle(GetWorkflowTemplatesQuery q, CancellationToken ct)
    {
        var templates = await unitOfWork.Repository<WorkflowTemplate>()
            .GetAllAsync(t => q.EntityType == null || t.EntityType == q.EntityType, ct);

        var templateIds = templates.Select(t => t.Id).ToList();
        var steps = await unitOfWork.Repository<WorkflowTemplateStep>()
            .GetAllAsync(s => templateIds.Contains(s.TemplateId), ct);

        // Bulk-fetch approver names — 1 query mỗi loại, tránh N+1
        var userApproverIds = steps
            .Where(s => s.ApproverType == WorkflowApproverType.SpecificUser && s.ApproverId.HasValue)
            .Select(s => s.ApproverId!.Value).Distinct().ToList();

        var roleApproverIds = steps
            .Where(s => s.ApproverType == WorkflowApproverType.Role && s.ApproverId.HasValue)
            .Select(s => s.ApproverId!.Value).Distinct().ToList();

        var userNames = userApproverIds.Count > 0
            ? (await unitOfWork.Repository<User>().GetAllAsync(u => userApproverIds.Contains(u.Id), ct))
              .ToDictionary(u => u.Id, u => u.FullName)
            : new Dictionary<Guid, string>();

        var roleNames = roleApproverIds.Count > 0
            ? (await unitOfWork.Repository<Role>().GetAllAsync(r => roleApproverIds.Contains(r.Id), ct))
              .ToDictionary(r => r.Id, r => !string.IsNullOrEmpty(r.DisplayName) ? r.DisplayName : r.RoleName)
            : new Dictionary<Guid, string>();

        var stepsByTemplate = steps.GroupBy(s => s.TemplateId)
            .ToDictionary(g => g.Key, g => g.OrderBy(s => s.StepOrder).ToList());

        return templates.OrderBy(t => t.EntityType).ThenBy(t => t.Name)
            .Select(t => new WorkflowTemplateResponse
            {
                Id = t.Id,
                Name = t.Name,
                EntityType = t.EntityType,
                IsActive = t.IsActive,
                ScopeType = t.ScopeType.ToString(),
                ScopeEntityId = t.ScopeEntityId,
                CreatedAt = t.CreatedAt,
                Steps = stepsByTemplate.TryGetValue(t.Id, out var s)
                    ? s.Select(x => new WorkflowTemplateStepResponse
                    {
                        Id = x.Id,
                        StepOrder = x.StepOrder,
                        StepName = x.StepName,
                        ApproverType = x.ApproverType.ToString(),
                        ApproverId = x.ApproverId,
                        ApproverName = x.ApproverType switch
                        {
                            WorkflowApproverType.SpecificUser => x.ApproverId.HasValue && userNames.TryGetValue(x.ApproverId.Value, out var un) ? un : null,
                            WorkflowApproverType.Role         => x.ApproverId.HasValue && roleNames.TryGetValue(x.ApproverId.Value, out var rn) ? rn : null,
                            _                                 => null,
                        },
                    }).ToList()
                    : [],
            }).ToList();
    }
}
