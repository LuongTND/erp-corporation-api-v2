namespace Application;

public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.ParentDepartmentName,
                opt => opt.MapFrom(s => s.ParentDepartment != null ? s.ParentDepartment.DepartmentName : null))
            .ForMember(d => d.ManagerName, opt => opt.MapFrom(s => s.Manager != null ? s.Manager.FullName : null));
    }
}