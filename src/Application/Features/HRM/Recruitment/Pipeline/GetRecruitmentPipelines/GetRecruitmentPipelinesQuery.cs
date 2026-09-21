namespace Application;

public sealed record GetRecruitmentPipelinesQuery : IRequest<IEnumerable<RecruitmentPipelineResponse>>;
