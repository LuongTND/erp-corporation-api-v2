namespace Application.DTOs.Roles;

public record CreateRoleRequest(
    string RoleName,
    string DisplayName,
    string? Description = null,
    bool BypassDataScope = false
);
