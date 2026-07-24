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
        var allRolePerms = await unitOfWork.Repository<RolePermission>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: rp => roleIds.Contains(rp.RoleId),
            ct: ct);

        var permIds = allRolePerms.Items.Select(rp => rp.PermissionId).Distinct().ToList();
        var allPerms = await unitOfWork.Repository<Permission>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: p => permIds.Contains(p.Id),
            ct: ct);

        var permMap = allPerms.Items.ToDictionary(p => p.Id);
        var rolePermMap = allRolePerms.Items
            .GroupBy(rp => rp.RoleId)
            .ToDictionary(g => g.Key, g => g
                .Where(rp => permMap.ContainsKey(rp.PermissionId))
                .Select(rp => new PermissionResponse
                {
                    Id = permMap[rp.PermissionId].Id,
                    PermissionCode = permMap[rp.PermissionId].PermissionCode,
                    Module = permMap[rp.PermissionId].Module.ToString()
                }));

        return roles.Select(r => new RoleResponse
        {
            Id = r.Id,
            RoleName = r.RoleName,
            Description = r.Description,
            IsSystemRole = r.IsSystemRole,
            Permissions = rolePermMap.TryGetValue(r.Id, out var perms) ? perms : []
        });
    }
}
