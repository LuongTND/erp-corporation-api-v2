using Role = Domain.Role;

namespace Infrastructure;

public static class RoleData
{
    public static IEnumerable<Role> GetRoles()
    {
        return
        [
            new Role { Id = GuidHelper.From(RoleConstants.Admin),   RoleName = RoleConstants.Admin,   IsSystemRole = true  },
        ];
    }
}
