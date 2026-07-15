namespace Application;

public record UpdateRoleRequest(
    string DisplayName,
    string? Description = null,
    bool BypassDataScope = false,
    bool IsActive = true
);