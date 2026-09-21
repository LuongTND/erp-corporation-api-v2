namespace Application;

public sealed class GetRecruitmentPipelinesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRecruitmentPipelinesQuery, IEnumerable<RecruitmentPipelineResponse>>
{
    public async Task<IEnumerable<RecruitmentPipelineResponse>> Handle(
        GetRecruitmentPipelinesQuery q, CancellationToken ct)
    {
        var pipelines = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
            .Query()
            .Include(p => p.Stages)
                .ThenInclude(s => s.RoundType)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync(ct);

        return pipelines.Select(p => p.Adapt<RecruitmentPipelineResponse>());
    }
}
