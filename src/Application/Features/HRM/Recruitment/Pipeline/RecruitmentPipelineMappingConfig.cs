namespace Application;

public sealed class RecruitmentPipelineMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.RecruitmentPipeline, RecruitmentPipelineResponse>()
            .Map(dest => dest.Stages, src => src.Stages
                .OrderBy(s => s.DisplayOrder));

        config.NewConfig<Domain.RecruitmentPipelineStage, RecruitmentPipelineStageResponse>()
            .Map(dest => dest.RoundTypeName, src => src.RoundType != null ? src.RoundType.Name : null);
    }
}
