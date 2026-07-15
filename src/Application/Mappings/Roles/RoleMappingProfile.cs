namespace Application;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Permission, PermissionDto>();

        CreateMap<Role, RoleDto>()
            .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.RolePermissions.Select(rp => rp.Permission)));
    }
}