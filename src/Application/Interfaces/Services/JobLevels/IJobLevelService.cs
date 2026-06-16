using Application.DTOs.JobLevels;
using Application.Interfaces.Services.Common;

namespace Application.Interfaces.Services.JobLevels;

public interface IJobLevelService : ICrudService<JobLevelDto, CreateJobLevelRequest, UpdateJobLevelRequest>;
