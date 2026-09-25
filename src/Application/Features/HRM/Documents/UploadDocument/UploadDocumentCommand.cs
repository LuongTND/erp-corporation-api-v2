namespace Application;

public sealed record UploadDocumentCommand(
    Guid UserId,
    DocumentCategory Category,
    string? CustomName,
    Stream FileStream,
    string ContentType,
    string OriginalFileName,
    long FileSizeBytes,
    DateTimeOffset? IssuedDate,
    DateTimeOffset? ExpiryDate,
    string? Notes,
    bool IsVisibleToEmployee = false,
    bool IsHrUpload = false          // true = HR route, bỏ category whitelist
) : IRequest<EmployeeDocumentResponse>;
