namespace Contract;

public sealed record WorkflowTaskResponse
{
    public Guid Id { get; init; }
    public Guid InstanceId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public int StepOrder { get; init; }
    public string StepName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Note { get; init; }
    public DateTimeOffset? ActedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
