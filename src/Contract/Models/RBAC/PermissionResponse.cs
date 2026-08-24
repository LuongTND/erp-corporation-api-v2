namespace Contract;

public sealed class PermissionResponse
{
    public Guid Id { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RoleCount { get; set; }
}
