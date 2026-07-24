using Role = Domain.Role;

namespace Infrastructure;

public static class RoleData
{
    public static IEnumerable<Role> GetRoles()
    {
        return
        [
            new Role { Id = GuidHelper.From(RoleConstants.Staff),   RoleName = RoleConstants.Staff,   IsSystemRole = false },
            new Role { Id = GuidHelper.From(RoleConstants.Manager), RoleName = RoleConstants.Manager, IsSystemRole = false },
            new Role { Id = GuidHelper.From(RoleConstants.Admin),   RoleName = RoleConstants.Admin,   IsSystemRole = true  },
        ];
    }
}
