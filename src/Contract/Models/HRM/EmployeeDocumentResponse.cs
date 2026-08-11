namespace Contract;

public sealed class EmployeeDocumentResponse
{
    public Guid Id { get; init; }
    public string Category { get; init; } = string.Empty;
    public string? CustomName { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string OriginalFileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public DateTimeOffset? IssuedDate { get; init; }
    public DateTimeOffset? ExpiryDate { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public bool IsExpired { get; init; }
    public bool IsExpiringSoon { get; init; }
}
