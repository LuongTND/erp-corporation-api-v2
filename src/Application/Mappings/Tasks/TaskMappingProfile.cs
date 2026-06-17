using Application.DTOs.Tasks;
using AutoMapper;
using Domain.Entities.Tasks;

namespace Application.Mappings.Tasks;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ForMember(d => d.KpiIds, opt => opt.MapFrom(s => s.TaskKpis.Select(k => k.KPIID).ToList()))
            .ForMember(d => d.LmsCourseIds, opt => opt.MapFrom(s => s.TaskLmsCourses.Select(c => c.CourseID).ToList()))
            .ForMember(d => d.Assignees, opt => opt.MapFrom(s => s.Assignees))
            .ForMember(d => d.Followers, opt => opt.MapFrom(s => s.Followers))
            .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments))
            .ForMember(d => d.ActivityLogs, opt => opt.MapFrom(s => s.ActivityLogs))
            .ForMember(d => d.Subtasks, opt => opt.MapFrom(s => s.Subtasks
                .Where(st => st.IsActive)
                .OrderBy(st => st.CreatedAt)));

        CreateMap<TaskAssignee, TaskAssigneeDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.User != null ? s.User.EmployeeCode : string.Empty));

        CreateMap<TaskFollower, TaskFollowerDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.User != null ? s.User.EmployeeCode : string.Empty));

        CreateMap<TaskComment, TaskCommentDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty))
            .ForMember(d => d.AvatarUrl, opt => opt.MapFrom(s => s.User != null ? s.User.AvatarUrl : null))
            .ForMember(d => d.Replies, opt => opt.MapFrom(s => s.Replies));

        CreateMap<TaskActivityLog, TaskActivityLogDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty));
    }
}
