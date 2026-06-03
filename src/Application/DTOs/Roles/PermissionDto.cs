using Domain.Enums;

namespace Application.DTOs.Roles;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string PermissionCode { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public PermissionModule Module { get; set; }
    public PermissionAction Action { get; set; }
    public string Resource { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
