namespace Application;

public sealed class GetMyPermissionsQueryHandler(
    IPermissionService permissionService,
    IUserContext userContext)
    : IRequestHandler<GetMyPermissionsQuery, IReadOnlyCollection<string>>
{
    public async Task<IReadOnlyCollection<string>> Handle(GetMyPermissionsQuery query, CancellationToken ct)
    {
        var permissions = await permissionService.GetPermissionsAsync(userContext.UserId);
        return [.. permissions];
    }
}
