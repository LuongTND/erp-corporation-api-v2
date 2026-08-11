namespace Domain;

public class EmployeeDocument : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DocumentCategory Category { get; set; }
    public string? CustomName { get; set; }

    public string BlobName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }

    public DateTimeOffset? IssuedDate { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }
    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}
