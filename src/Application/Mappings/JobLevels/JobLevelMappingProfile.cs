using Application.DTOs.JobLevels;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings.JobLevels;

public class JobLevelMappingProfile : Profile
{
    public JobLevelMappingProfile()
    {
        CreateMap<JobLevel, JobLevelDto>();
    }
}
