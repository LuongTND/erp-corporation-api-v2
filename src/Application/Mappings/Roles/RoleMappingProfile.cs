using Application.DTOs.Roles;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings.Roles;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Permission, PermissionDto>();

        CreateMap<Role, RoleDto>()
            .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.RolePermissions.Select(rp => rp.Permission)));
    }
}
