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
                    }).ToList()
                    : [],
            }).ToList();
    }
}
