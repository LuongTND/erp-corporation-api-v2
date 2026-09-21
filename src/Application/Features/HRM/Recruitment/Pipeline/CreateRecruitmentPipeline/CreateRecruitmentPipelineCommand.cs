namespace Application;

public sealed record CreateRecruitmentPipelineCommand(string Name, bool IsDefault) : IRequest<Guid>;
