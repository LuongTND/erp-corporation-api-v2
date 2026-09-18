namespace Application;

public sealed class GetRolesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRolesQuery, IEnumerable<RoleResponse>>
{
    public async Task<IEnumerable<RoleResponse>> Handle(GetRolesQuery query, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<Role>().GetPagedAsync(
            new QueryInfo { Top = 100, NeedTotalCount = false },
            orderBy: q => q.OrderBy(r => r.RoleName),
            ct: ct);

        var roles = result.Items.ToList();

        var roleIds = roles.Select(r => r.Id).ToList();
        var allRolePerms = await unitOfWork.Repository<RolePermission>().GetAllAsync(
            rp => roleIds.Contains(rp.RoleId), ct);

        var permIds = allRolePerms.Select(rp => rp.PermissionId).Distinct().ToList();
        var allPerms = permIds.Count == 0
            ? []
            : await unitOfWork.Repository<Permission>().GetAllAsync(p => permIds.Contains(p.Id), ct);

        var permMap = allPerms.ToDictionary(p => p.Id);
        var rolePermMap = allRolePerms
            .GroupBy(rp => rp.RoleId)
            .ToDictionary(g => g.Key, g => g
                .Where(rp => permMap.ContainsKey(rp.PermissionId))
                .Select(rp => new PermissionResponse
                {
                    Id = permMap[rp.PermissionId].Id,
                    PermissionCode = permMap[rp.PermissionId].PermissionCode,
                    PermissionName = permMap[rp.PermissionId].PermissionName
                }));

        var userRoleCounts = (await unitOfWork.Repository<UserRole>().GetAllAsync(
                ur => roleIds.Contains(ur.RoleId) && ur.IsActive, ct))
            .GroupBy(ur => ur.RoleId)
            .ToDictionary(g => g.Key, g => g.Count());

        return roles.Select(r => new RoleResponse
        {
            Id = r.Id,
            RoleName = r.RoleName,
            DisplayName = r.DisplayName,
            Description = r.Description,
            IsSystemRole = r.IsSystemRole,
            DefaultDataScope = r.DefaultDataScope.ToString(),
            Permissions = rolePermMap.TryGetValue(r.Id, out var perms) ? perms : [],
            UserCount = userRoleCounts.TryGetValue(r.Id, out var count) ? count : 0
        });
    }
}
