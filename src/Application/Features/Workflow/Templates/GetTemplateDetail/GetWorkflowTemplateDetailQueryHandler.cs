namespace Application;

public sealed class GetWorkflowTemplateDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWorkflowTemplateDetailQuery, WorkflowTemplateResponse>
{
    public async Task<WorkflowTemplateResponse> Handle(GetWorkflowTemplateDetailQuery q, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<WorkflowTemplate>()
            .FindAsync(t => t.Id == q.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", q.TemplateId));

        var steps = await unitOfWork.Repository<WorkflowTemplateStep>()
            .GetAllAsync(s => s.TemplateId == template.Id, ct);

        // Bulk-fetch tên approver (cùng pattern với GetWorkflowTemplatesQueryHandler)
        var userIds = steps
            .Where(s => s.ApproverType == WorkflowApproverType.SpecificUser && s.ApproverId.HasValue)
            .Select(s => s.ApproverId!.Value).Distinct().ToList();

        var roleIds = steps
            .Where(s => s.ApproverType == WorkflowApproverType.Role && s.ApproverId.HasValue)
            .Select(s => s.ApproverId!.Value).Distinct().ToList();

        var userNames = userIds.Count > 0
            ? (await unitOfWork.Repository<User>().GetAllAsync(u => userIds.Contains(u.Id), ct))
              .ToDictionary(u => u.Id, u => u.FullName)
            : new Dictionary<Guid, string>();

        var roleNames = roleIds.Count > 0
            ? (await unitOfWork.Repository<Role>().GetAllAsync(r => roleIds.Contains(r.Id), ct))
              .ToDictionary(r => r.Id, r => !string.IsNullOrEmpty(r.DisplayName) ? r.DisplayName : r.RoleName)
            : new Dictionary<Guid, string>();

        return new WorkflowTemplateResponse
        {
            Id = template.Id,
            Name = template.Name,
            EntityType = template.EntityType,
            IsActive = template.IsActive,
            ScopeType = template.ScopeType.ToString(),
            ScopeEntityId = template.ScopeEntityId,
            CreatedAt = template.CreatedAt,
            Steps = steps.OrderBy(s => s.StepOrder).Select(x => new WorkflowTemplateStepResponse
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
            }).ToList(),
        };
    }
}
