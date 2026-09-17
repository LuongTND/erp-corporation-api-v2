namespace Application;

public sealed class GetWorkflowEntityTypesQueryHandler
    : IRequestHandler<GetWorkflowEntityTypesQuery, IReadOnlyList<WorkflowEntityTypeItem>>
{
    public Task<IReadOnlyList<WorkflowEntityTypeItem>> Handle(GetWorkflowEntityTypesQuery _, CancellationToken ct)
        => Task.FromResult(WorkflowEntityTypes.All);
}
