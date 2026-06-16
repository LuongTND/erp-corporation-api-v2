namespace Application.DTOs.Roles;

public class RoleDto
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool BypassDataScope { get; set; }
    public bool IsActive { get; set; }
    public List<PermissionDto> Permissions { get; set; } = [];
}
