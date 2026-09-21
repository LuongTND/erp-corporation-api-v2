namespace Application;

public sealed record RemovePipelineStageCommand(Guid StageId) : IRequest<Unit>;
