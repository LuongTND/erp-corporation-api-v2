
namespace Application;
public class JobLevelMappingProfile : Profile
{
    public JobLevelMappingProfile()
    {
        CreateMap<JobLevel, JobLevelDto>();
    }
}
