namespace Application.DTOs.Roles;

public record UpdateRolePermissionsRequest(List<Guid> PermissionIds);
