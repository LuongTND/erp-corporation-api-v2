using Application.DTOs.Users;
using AutoMapper;

namespace Application.Mappings.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : string.Empty))
            .ForMember(d => d.JobLevelName, opt => opt.MapFrom(s => s.JobLevel != null ? s.JobLevel.LevelName : string.Empty))
            .ForMember(d => d.ManagerName, opt => opt.MapFrom(s => s.Manager != null ? s.Manager.FullName : null))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles
                .Where(ur => ur.IsActive && ur.RevokedAt == null && ur.Role.IsActive)
                .Select(ur => ur.Role.RoleName)
                .ToList()));

        CreateMap<UserDepartment, UserDepartmentDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : string.Empty))
            .ForMember(d => d.DepartmentCode, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentCode : string.Empty));
    }
}
