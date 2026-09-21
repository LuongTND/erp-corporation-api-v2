namespace Application;

public sealed record DeleteRecruitmentPipelineCommand(Guid Id) : IRequest<Unit>;
