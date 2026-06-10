using Application.DTOs.JobLevels;
using AutoMapper;

namespace Application.Mappings.JobLevels;

public class JobLevelMappingProfile : Profile
{
    public JobLevelMappingProfile()
    {
        CreateMap<JobLevel, JobLevelDto>();
    }
}
