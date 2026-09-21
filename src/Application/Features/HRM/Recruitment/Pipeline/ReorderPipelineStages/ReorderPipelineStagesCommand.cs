namespace Application;

public sealed record ReorderPipelineStagesCommand(
    Guid PipelineId,
    IEnumerable<ReorderPipelineStageItem> Items
) : IRequest<Unit>;

public sealed record ReorderPipelineStageItem(Guid StageId, int DisplayOrder);
