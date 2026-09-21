namespace Application;

public sealed record UpdatePipelineStageCommand(Guid StageId, string Name) : IRequest<Unit>;
