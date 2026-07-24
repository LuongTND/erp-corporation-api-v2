namespace Application;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(Guid userId);
    Task InvalidateCacheAsync(Guid roleId);
}
