namespace Contract;

public sealed class PermissionResponse
{
    public Guid Id { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RoleCount { get; set; }

    // Parsed from PermissionCode format: "module:resource:action"
    public string Module   => PermissionCode.Split(':') is [var m, ..] ? m : string.Empty;
    public string Resource => PermissionCode.Split(':') is [_, var r, ..] ? r : string.Empty;
    public string Action   => PermissionCode.Split(':') is [_, _, var a] ? a : string.Empty;
}
