namespace Contract;

public sealed class RoleResponse
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public IEnumerable<PermissionResponse> Permissions { get; set; } = [];
}
