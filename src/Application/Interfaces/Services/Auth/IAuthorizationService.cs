namespace Application;
public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default);
    Task EnsurePermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default);
}
