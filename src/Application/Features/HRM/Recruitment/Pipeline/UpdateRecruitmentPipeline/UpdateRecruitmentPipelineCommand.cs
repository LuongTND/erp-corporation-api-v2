namespace Application;

public sealed record UpdateRecruitmentPipelineCommand(Guid Id, string Name, bool IsDefault) : IRequest<Unit>;
