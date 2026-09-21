namespace Application;

public sealed record AddPipelineStageCommand(
    Guid PipelineId,
    Guid RoundTypeId,
    string? Name,
    int DisplayOrder
) : IRequest<Guid>;
