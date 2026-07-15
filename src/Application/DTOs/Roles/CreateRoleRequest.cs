namespace Application;

public record CreateRoleRequest(
    string RoleName,
    string DisplayName,
    string? Description = null,
    bool BypassDataScope = false
);