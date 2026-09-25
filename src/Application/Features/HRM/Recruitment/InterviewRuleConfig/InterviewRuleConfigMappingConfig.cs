namespace Application;

public sealed class InterviewRuleConfigMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.InterviewRuleConfig, InterviewRuleConfigResponse>()
            .Map(dest => dest.Context, src => src.Context.ToString())
            .Map(dest => dest.Location, src => src.Location.ToString())
            .Map(dest => dest.RegionName, src => src.Region != null ? src.Region.Name : null)
            .Map(dest => dest.DepartmentName, src => src.Department != null ? src.Department.DepartmentName : null);
    }
}
