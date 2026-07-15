namespace Application;

public record CreatePermissionRequest(
    string PermissionCode,
    string PermissionName,
    PermissionModule Module,
    PermissionAction Action,
    string Resource,
    string? Description = null);

public record UpdatePermissionRequest(
    string PermissionName,
    string? Description = null,
    bool IsActive = true);