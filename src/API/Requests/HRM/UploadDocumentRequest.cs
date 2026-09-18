namespace API;

public sealed class UploadDocumentRequest
{
    public IFormFile File { get; set; } = default!;
    public DocumentCategory Category { get; set; }
    public string? CustomName { get; set; }
    public DateTimeOffset? IssuedDate { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public bool IsVisibleToEmployee { get; set; }
}
