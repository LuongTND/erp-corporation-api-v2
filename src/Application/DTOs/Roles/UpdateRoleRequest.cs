namespace Application.DTOs.Roles;

public record UpdateRoleRequest(
    string DisplayName,
    string? Description = null,
    bool BypassDataScope = false,
    bool IsActive = true
);
