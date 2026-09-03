namespace Contract;

public sealed record WorkflowTemplateResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string ScopeType { get; init; } = string.Empty;
    public Guid? ScopeEntityId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<WorkflowTemplateStepResponse> Steps { get; init; } = [];
}

public sealed record WorkflowTemplateStepResponse
{
    public Guid Id { get; init; }
    public int StepOrder { get; init; }
    public string StepName { get; init; } = string.Empty;
    public string ApproverType { get; init; } = string.Empty;
    public Guid? ApproverId { get; init; }
}
