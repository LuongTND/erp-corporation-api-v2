using API.Base;
using Application.DTOs.JobLevels;
using Application.Interfaces.Services.JobLevels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.JobLevels;

[Route("api/job-levels")]
public class JobLevelsController : CrudApiController<IJobLevelService, JobLevelDto, CreateJobLevelRequest, UpdateJobLevelRequest>
{
    public JobLevelsController(IJobLevelService jobLevelService)
        : base(jobLevelService, new CrudPermissions
        {
            Read = "hrm.joblevel.read",
            Create = "hrm.joblevel.create",
            Update = "hrm.joblevel.update",
            Delete = "hrm.joblevel.delete"
        })
    {
    }
}
