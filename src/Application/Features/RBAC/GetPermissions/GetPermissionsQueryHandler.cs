namespace Application;

public sealed class GetPermissionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPermissionsQuery, QueryResult<PermissionResponse>>
{
    public async Task<QueryResult<PermissionResponse>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        var allPermissions = await unitOfWork.Repository<Permission>().GetAllAsync(_ => true, ct);
        var allRolePerms = await unitOfWork.Repository<RolePermission>().GetAllAsync(_ => true, ct);

        var roleCountMap = allRolePerms
            .GroupBy(rp => rp.PermissionId)
            .ToDictionary(g => g.Key, g => g.Count());

        IEnumerable<Permission> filtered = allPermissions;
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var search = query.SearchText.Trim().ToLowerInvariant();
            filtered = filtered.Where(p =>
                p.PermissionCode.ToLowerInvariant().Contains(search) ||
                p.PermissionName.ToLowerInvariant().Contains(search) ||
                (p.Description?.ToLowerInvariant().Contains(search) ?? false));
        }

        var ordered = filtered.OrderBy(p => p.PermissionCode).ToList();
        var totalCount = ordered.Count;

        var items = (query.Top > 0 ? ordered.Skip(query.Skip).Take(query.Top) : ordered)
            .Select(p => new PermissionResponse
            {
                Id = p.Id,
                PermissionCode = p.PermissionCode,
                PermissionName = p.PermissionName,
                Description = p.Description,
                RoleCount = roleCountMap.GetValueOrDefault(p.Id, 0)
            });

        return new QueryResult<PermissionResponse> { Items = items, TotalCount = totalCount };
    }
}
