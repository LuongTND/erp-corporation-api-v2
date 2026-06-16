using Application.Common.Exceptions;
using Application.Interfaces.Repositories.Roles;
using Application.Interfaces.Services.Auth;

namespace Infrastructure.Implementations.Services.Auth;

public class AuthorizationService : IAuthorizationService
{
    private readonly IRoleRepository _roleRepository;
    private readonly Dictionary<string, bool> _permissionCache = new();

    public AuthorizationService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default)
    {
        var cacheKey = $"{userId}:{permissionCode}";
        if (_permissionCache.TryGetValue(cacheKey, out var cached))
            return cached;

        var hasPermission = await _roleRepository.HasPermissionAsync(userId, permissionCode, ct);
        _permissionCache[cacheKey] = hasPermission;
        return hasPermission;
    }

    public async Task EnsurePermissionAsync(Guid userId, string permissionCode, CancellationToken ct = default)
    {
        var hasPermission = await HasPermissionAsync(userId, permissionCode, ct);
        if (!hasPermission)
        {
            throw new ForbiddenException($"Bạn không có quyền thực hiện hành động: '{permissionCode}'");
        }
    }
}
