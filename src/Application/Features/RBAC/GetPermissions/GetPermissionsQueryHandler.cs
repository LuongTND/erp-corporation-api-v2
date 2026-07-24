namespace Application;

public sealed class GetPermissionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionResponse>>
{
    public async Task<IEnumerable<PermissionResponse>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<Permission>().GetPagedAsync(
            new QueryInfo { Top = 500, NeedTotalCount = false },
            orderBy: q => q.OrderBy(p => p.Module).ThenBy(p => p.PermissionCode),
            ct: ct);

        return result.Items.Select(p => new PermissionResponse
        {
            Id = p.Id,
            PermissionCode = p.PermissionCode,
            Module = p.Module.ToString(),
            Description = p.Description
        });
    }
}
