namespace Contract;

public sealed class PermissionResponse
{
    public Guid Id { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
}
